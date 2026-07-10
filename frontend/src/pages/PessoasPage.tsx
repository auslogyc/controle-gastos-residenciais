// Página de gerenciamento de Pessoas
import { useState, useEffect, useCallback } from 'react';
import type { Pessoa } from '../types';
import { listarPessoas, criarPessoa, deletarPessoa } from '../services/api';

export default function PessoasPage() {
  // Estado da lista de pessoas
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  // Estados do formulário
  const [nome, setNome] = useState('');
  const [idade, setIdade] = useState('');
  // Estados de UI
  const [carregando, setCarregando] = useState(false);
  const [enviando, setEnviando] = useState(false);
  const [mensagem, setMensagem] = useState<{ tipo: 'sucesso' | 'erro'; texto: string } | null>(null);
  // Estado do modal de confirmação de exclusão
  const [pessoaParaDeletar, setPessoaParaDeletar] = useState<Pessoa | null>(null);

  // Limpa a mensagem de toast após 4 segundos
  useEffect(() => {
    if (mensagem) {
      const timer = setTimeout(() => setMensagem(null), 4000);
      return () => clearTimeout(timer);
    }
  }, [mensagem]);

  // Carrega a lista de pessoas do backend
  const carregarPessoas = useCallback(async () => {
    setCarregando(true);
    try {
      const dados = await listarPessoas();
      setPessoas(dados);
    } catch (erro) {
      setMensagem({ tipo: 'erro', texto: erro instanceof Error ? erro.message : 'Erro ao carregar pessoas' });
    } finally {
      setCarregando(false);
    }
  }, []);

  // Carrega as pessoas ao montar o componente
  useEffect(() => {
    carregarPessoas();
  }, [carregarPessoas]);

  // Submete o formulário de criação de pessoa
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validação do formulário
    if (!nome.trim()) {
      setMensagem({ tipo: 'erro', texto: 'O nome é obrigatório.' });
      return;
    }
    const idadeNum = Number(idade);
    if (isNaN(idadeNum) || idadeNum < 0) {
      setMensagem({ tipo: 'erro', texto: 'A idade deve ser um número maior ou igual a zero.' });
      return;
    }

    setEnviando(true);
    try {
      await criarPessoa({ nome: nome.trim(), idade: idadeNum });
      setMensagem({ tipo: 'sucesso', texto: `Pessoa "${nome.trim()}" cadastrada com sucesso!` });
      setNome('');
      setIdade('');
      await carregarPessoas();
    } catch (erro) {
      setMensagem({ tipo: 'erro', texto: erro instanceof Error ? erro.message : 'Erro ao cadastrar pessoa' });
    } finally {
      setEnviando(false);
    }
  };

  // Confirma a exclusão de uma pessoa
  const confirmarExclusao = async () => {
    if (!pessoaParaDeletar) return;
    try {
      await deletarPessoa(pessoaParaDeletar.id);
      setMensagem({ tipo: 'sucesso', texto: `Pessoa "${pessoaParaDeletar.nome}" removida com sucesso!` });
      setPessoaParaDeletar(null);
      await carregarPessoas();
    } catch (erro) {
      setMensagem({ tipo: 'erro', texto: erro instanceof Error ? erro.message : 'Erro ao deletar pessoa' });
      setPessoaParaDeletar(null);
    }
  };

  return (
    <div className="page-container">
      {/* Toast de notificação */}
      {mensagem && (
        <div className={`toast toast-${mensagem.tipo}`}>
          <span className="toast-icon">{mensagem.tipo === 'sucesso' ? '✓' : '✕'}</span>
          {mensagem.texto}
        </div>
      )}

      {/* Modal de confirmação de exclusão */}
      {pessoaParaDeletar && (
        <div className="modal-overlay" onClick={() => setPessoaParaDeletar(null)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h3>Confirmar Exclusão</h3>
            <p>
              Tem certeza que deseja excluir <strong>{pessoaParaDeletar.nome}</strong>?
              <br />
              <span className="modal-warning">Esta ação não pode ser desfeita.</span>
            </p>
            <div className="modal-actions">
              <button className="btn btn-secondary" onClick={() => setPessoaParaDeletar(null)}>
                Cancelar
              </button>
              <button className="btn btn-danger" onClick={confirmarExclusao}>
                Excluir
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Cabeçalho da página */}
      <div className="page-header">
        <h1>
          <span className="page-icon">👥</span>
          Pessoas
        </h1>
        <p className="page-subtitle">Gerencie as pessoas do seu controle financeiro</p>
      </div>

      {/* Formulário de cadastro */}
      <div className="card">
        <h2 className="card-title">Cadastrar Nova Pessoa</h2>
        <form onSubmit={handleSubmit} className="form">
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="nome">Nome</label>
              <input
                id="nome"
                type="text"
                placeholder="Digite o nome completo"
                value={nome}
                onChange={(e) => setNome(e.target.value)}
                disabled={enviando}
              />
            </div>
            <div className="form-group">
              <label htmlFor="idade">Idade</label>
              <input
                id="idade"
                type="number"
                placeholder="Ex: 25"
                value={idade}
                onChange={(e) => setIdade(e.target.value)}
                min="0"
                disabled={enviando}
              />
            </div>
          </div>
          <button type="submit" className="btn btn-primary" disabled={enviando}>
            {enviando ? (
              <>
                <span className="spinner" /> Cadastrando...
              </>
            ) : (
              '+ Cadastrar Pessoa'
            )}
          </button>
        </form>
      </div>

      {/* Tabela de pessoas */}
      <div className="card">
        <h2 className="card-title">
          Pessoas Cadastradas
          <span className="badge">{pessoas.length}</span>
        </h2>

        {carregando ? (
          <div className="loading-state">
            <div className="spinner-large" />
            <p>Carregando pessoas...</p>
          </div>
        ) : pessoas.length === 0 ? (
          <div className="empty-state">
            <span className="empty-icon">📋</span>
            <p>Nenhuma pessoa cadastrada ainda.</p>
            <p className="empty-hint">Use o formulário acima para adicionar a primeira pessoa.</p>
          </div>
        ) : (
          <div className="table-wrapper">
            <table>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Nome</th>
                  <th>Idade</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                {pessoas.map((pessoa) => (
                  <tr key={pessoa.id}>
                    <td className="td-id">#{pessoa.id}</td>
                    <td>{pessoa.nome}</td>
                    <td>{pessoa.idade} anos</td>
                    <td>
                      <button
                        className="btn btn-danger btn-sm"
                        onClick={() => setPessoaParaDeletar(pessoa)}
                      >
                        🗑️ Excluir
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}

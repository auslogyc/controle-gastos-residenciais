// Página de gerenciamento de Transações
import { useState, useEffect, useCallback } from 'react';
import type { Transacao, Pessoa } from '../types';
import { TipoTransacao } from '../types';
import { listarTransacoes, criarTransacao, listarPessoas } from '../services/api';

// Formata valor para moeda brasileira (BRL)
function formatarMoeda(valor: number): string {
  return valor.toLocaleString('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  });
}

export default function TransacoesPage() {
  // Estado da lista de transações e pessoas
  const [transacoes, setTransacoes] = useState<Transacao[]>([]);
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  // Estados do formulário
  const [descricao, setDescricao] = useState('');
  const [valor, setValor] = useState('');
  const [tipo, setTipo] = useState<TipoTransacao>(TipoTransacao.Despesa);
  const [pessoaId, setPessoaId] = useState('');
  // Estados de UI
  const [carregando, setCarregando] = useState(false);
  const [enviando, setEnviando] = useState(false);
  const [mensagem, setMensagem] = useState<{ tipo: 'sucesso' | 'erro'; texto: string } | null>(null);

  // Limpa a mensagem de toast após 4 segundos
  useEffect(() => {
    if (mensagem) {
      const timer = setTimeout(() => setMensagem(null), 4000);
      return () => clearTimeout(timer);
    }
  }, [mensagem]);

  // Carrega transações e pessoas do backend
  const carregarDados = useCallback(async () => {
    setCarregando(true);
    try {
      const [dadosTransacoes, dadosPessoas] = await Promise.all([
        listarTransacoes(),
        listarPessoas(),
      ]);
      setTransacoes(dadosTransacoes);
      setPessoas(dadosPessoas);
    } catch (erro) {
      setMensagem({ tipo: 'erro', texto: erro instanceof Error ? erro.message : 'Erro ao carregar dados' });
    } finally {
      setCarregando(false);
    }
  }, []);

  // Carrega os dados ao montar o componente
  useEffect(() => {
    carregarDados();
  }, [carregarDados]);

  // Submete o formulário de criação de transação
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validações do formulário
    if (!descricao.trim()) {
      setMensagem({ tipo: 'erro', texto: 'A descrição é obrigatória.' });
      return;
    }
    const valorNum = Number(valor);
    if (isNaN(valorNum) || valorNum <= 0) {
      setMensagem({ tipo: 'erro', texto: 'O valor deve ser um número positivo.' });
      return;
    }
    if (!pessoaId) {
      setMensagem({ tipo: 'erro', texto: 'Selecione uma pessoa.' });
      return;
    }

    setEnviando(true);
    try {
      await criarTransacao({
        descricao: descricao.trim(),
        valor: valorNum,
        tipo,
        pessoaId: Number(pessoaId),
      });
      setMensagem({ tipo: 'sucesso', texto: 'Transação registrada com sucesso!' });
      // Limpa o formulário
      setDescricao('');
      setValor('');
      setTipo(TipoTransacao.Despesa);
      setPessoaId('');
      await carregarDados();
    } catch (erro) {
      // Mostra erro do backend (ex: validação de idade menor)
      setMensagem({ tipo: 'erro', texto: erro instanceof Error ? erro.message : 'Erro ao registrar transação' });
    } finally {
      setEnviando(false);
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

      {/* Cabeçalho da página */}
      <div className="page-header">
        <h1>
          <span className="page-icon">💸</span>
          Transações
        </h1>
        <p className="page-subtitle">Registre receitas e despesas para cada pessoa</p>
      </div>

      {/* Formulário de cadastro de transação */}
      <div className="card">
        <h2 className="card-title">Nova Transação</h2>
        <form onSubmit={handleSubmit} className="form">
          <div className="form-row">
            <div className="form-group form-group-grow">
              <label htmlFor="descricao">Descrição</label>
              <input
                id="descricao"
                type="text"
                placeholder="Ex: Conta de luz"
                value={descricao}
                onChange={(e) => setDescricao(e.target.value)}
                disabled={enviando}
              />
            </div>
            <div className="form-group">
              <label htmlFor="valor">Valor (R$)</label>
              <input
                id="valor"
                type="number"
                placeholder="0,00"
                value={valor}
                onChange={(e) => setValor(e.target.value)}
                min="0.01"
                step="0.01"
                disabled={enviando}
              />
            </div>
          </div>
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="tipo">Tipo</label>
              <select
                id="tipo"
                value={tipo}
                onChange={(e) => setTipo(Number(e.target.value) as TipoTransacao)}
                disabled={enviando}
              >
                <option value={TipoTransacao.Despesa}>🔴 Despesa</option>
                <option value={TipoTransacao.Receita}>🟢 Receita</option>
              </select>
            </div>
            <div className="form-group form-group-grow">
              <label htmlFor="pessoa">Pessoa</label>
              <select
                id="pessoa"
                value={pessoaId}
                onChange={(e) => setPessoaId(e.target.value)}
                disabled={enviando}
              >
                <option value="">Selecione uma pessoa...</option>
                {pessoas.map((pessoa) => (
                  <option key={pessoa.id} value={pessoa.id}>
                    {pessoa.nome} ({pessoa.idade} anos)
                  </option>
                ))}
              </select>
            </div>
          </div>
          <button type="submit" className="btn btn-primary" disabled={enviando}>
            {enviando ? (
              <>
                <span className="spinner" /> Registrando...
              </>
            ) : (
              '+ Registrar Transação'
            )}
          </button>
        </form>
      </div>

      {/* Tabela de transações */}
      <div className="card">
        <h2 className="card-title">
          Histórico de Transações
          <span className="badge">{transacoes.length}</span>
        </h2>

        {carregando ? (
          <div className="loading-state">
            <div className="spinner-large" />
            <p>Carregando transações...</p>
          </div>
        ) : transacoes.length === 0 ? (
          <div className="empty-state">
            <span className="empty-icon">📊</span>
            <p>Nenhuma transação registrada ainda.</p>
            <p className="empty-hint">Use o formulário acima para registrar a primeira transação.</p>
          </div>
        ) : (
          <div className="table-wrapper">
            <table>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Descrição</th>
                  <th>Pessoa</th>
                  <th>Tipo</th>
                  <th>Valor</th>
                </tr>
              </thead>
              <tbody>
                {transacoes.map((t) => (
                  <tr key={t.id}>
                    <td className="td-id">#{t.id}</td>
                    <td>{t.descricao}</td>
                    <td>{t.pessoaNome}</td>
                    <td>
                      <span className={`tipo-badge tipo-${t.tipo === TipoTransacao.Receita ? 'receita' : 'despesa'}`}>
                        {t.tipo === TipoTransacao.Receita ? '↑ Receita' : '↓ Despesa'}
                      </span>
                    </td>
                    <td className={`valor-cell ${t.tipo === TipoTransacao.Receita ? 'valor-positivo' : 'valor-negativo'}`}>
                      {t.tipo === TipoTransacao.Receita ? '+' : '-'}{formatarMoeda(t.valor)}
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

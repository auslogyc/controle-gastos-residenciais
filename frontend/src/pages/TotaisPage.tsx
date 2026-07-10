// Página de visualização de Totais por pessoa e geral
import { useState, useEffect, useCallback } from 'react';
import type { TotaisGeral } from '../types';
import { buscarTotais } from '../services/api';

// Formata valor para moeda brasileira (BRL)
function formatarMoeda(valor: number): string {
  return valor.toLocaleString('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  });
}

export default function TotaisPage() {
  // Estado dos totais
  const [totais, setTotais] = useState<TotaisGeral | null>(null);
  // Estados de UI
  const [carregando, setCarregando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);

  // Busca os totais do backend
  const carregarTotais = useCallback(async () => {
    setCarregando(true);
    setErro(null);
    try {
      const dados = await buscarTotais();
      setTotais(dados);
    } catch (error) {
      setErro(error instanceof Error ? error.message : 'Erro ao carregar totais');
    } finally {
      setCarregando(false);
    }
  }, []);

  // Carrega os totais ao montar o componente (auto-refresh)
  useEffect(() => {
    carregarTotais();
  }, [carregarTotais]);

  return (
    <div className="page-container">
      {/* Cabeçalho da página */}
      <div className="page-header">
        <h1>
          <span className="page-icon">📈</span>
          Totais
        </h1>
        <p className="page-subtitle">Visão geral de receitas, despesas e saldo</p>
      </div>

      {/* Cards de resumo geral */}
      {totais && (
        <div className="totais-cards">
          <div className="totais-card totais-receita">
            <span className="totais-card-label">Total de Receitas</span>
            <span className="totais-card-valor">{formatarMoeda(totais.totalGeralReceitas)}</span>
          </div>
          <div className="totais-card totais-despesa">
            <span className="totais-card-label">Total de Despesas</span>
            <span className="totais-card-valor">{formatarMoeda(totais.totalGeralDespesas)}</span>
          </div>
          <div className={`totais-card ${totais.saldoGeral >= 0 ? 'totais-saldo-positivo' : 'totais-saldo-negativo'}`}>
            <span className="totais-card-label">Saldo Geral</span>
            <span className="totais-card-valor">{formatarMoeda(totais.saldoGeral)}</span>
          </div>
        </div>
      )}

      {/* Tabela detalhada por pessoa */}
      <div className="card">
        <div className="card-title-row">
          <h2 className="card-title">Detalhamento por Pessoa</h2>
          <button className="btn btn-secondary btn-sm" onClick={carregarTotais} disabled={carregando}>
            🔄 Atualizar
          </button>
        </div>

        {carregando ? (
          <div className="loading-state">
            <div className="spinner-large" />
            <p>Carregando totais...</p>
          </div>
        ) : erro ? (
          <div className="error-state">
            <span className="error-icon">⚠️</span>
            <p>{erro}</p>
            <button className="btn btn-primary btn-sm" onClick={carregarTotais}>
              Tentar novamente
            </button>
          </div>
        ) : !totais || totais.pessoas.length === 0 ? (
          <div className="empty-state">
            <span className="empty-icon">📊</span>
            <p>Nenhum dado disponível.</p>
            <p className="empty-hint">Cadastre pessoas e registre transações para ver os totais.</p>
          </div>
        ) : (
          <div className="table-wrapper">
            <table>
              <thead>
                <tr>
                  <th>Pessoa</th>
                  <th>Receitas</th>
                  <th>Despesas</th>
                  <th>Saldo</th>
                </tr>
              </thead>
              <tbody>
                {totais.pessoas.map((p) => (
                  <tr key={p.pessoaId}>
                    <td className="td-nome">{p.nome}</td>
                    <td className="valor-positivo">{formatarMoeda(p.totalReceitas)}</td>
                    <td className="valor-negativo">{formatarMoeda(p.totalDespesas)}</td>
                    <td className={p.saldo >= 0 ? 'valor-positivo' : 'valor-negativo'}>
                      <strong>{formatarMoeda(p.saldo)}</strong>
                    </td>
                  </tr>
                ))}
              </tbody>
              {/* Rodapé com totais gerais */}
              <tfoot>
                <tr className="totais-footer">
                  <td><strong>TOTAL GERAL</strong></td>
                  <td className="valor-positivo">
                    <strong>{formatarMoeda(totais.totalGeralReceitas)}</strong>
                  </td>
                  <td className="valor-negativo">
                    <strong>{formatarMoeda(totais.totalGeralDespesas)}</strong>
                  </td>
                  <td className={totais.saldoGeral >= 0 ? 'valor-positivo' : 'valor-negativo'}>
                    <strong>{formatarMoeda(totais.saldoGeral)}</strong>
                  </td>
                </tr>
              </tfoot>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}

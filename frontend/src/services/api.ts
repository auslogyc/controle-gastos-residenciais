// Serviço de comunicação com a API do backend
import type {
  Pessoa,
  CriarPessoaDto,
  Transacao,
  CriarTransacaoDto,
  TotaisGeral,
} from '../types';

// URL base da API do backend
const BASE_URL = 'http://localhost:5062/api';

// Função auxiliar para tratar erros da resposta HTTP
async function tratarResposta<T>(resposta: Response): Promise<T> {
  if (!resposta.ok) {
    // Tenta extrair a mensagem de erro do corpo da resposta
    const textoErro = await resposta.text();
    throw new Error(textoErro || `Erro ${resposta.status}: ${resposta.statusText}`);
  }
  return resposta.json();
}

// Função auxiliar para tratar respostas sem corpo (void)
async function tratarRespostaSemCorpo(resposta: Response): Promise<void> {
  if (!resposta.ok) {
    const textoErro = await resposta.text();
    throw new Error(textoErro || `Erro ${resposta.status}: ${resposta.statusText}`);
  }
}

// --- Endpoints de Pessoas ---

// Lista todas as pessoas cadastradas
export async function listarPessoas(): Promise<Pessoa[]> {
  const resposta = await fetch(`${BASE_URL}/pessoas`);
  return tratarResposta<Pessoa[]>(resposta);
}

// Cria uma nova pessoa
export async function criarPessoa(dto: CriarPessoaDto): Promise<Pessoa> {
  const resposta = await fetch(`${BASE_URL}/pessoas`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return tratarResposta<Pessoa>(resposta);
}

// Deleta uma pessoa pelo ID
export async function deletarPessoa(id: number): Promise<void> {
  const resposta = await fetch(`${BASE_URL}/pessoas/${id}`, {
    method: 'DELETE',
  });
  return tratarRespostaSemCorpo(resposta);
}

// --- Endpoints de Transações ---

// Lista todas as transações cadastradas
export async function listarTransacoes(): Promise<Transacao[]> {
  const resposta = await fetch(`${BASE_URL}/transacoes`);
  return tratarResposta<Transacao[]>(resposta);
}

// Cria uma nova transação
export async function criarTransacao(dto: CriarTransacaoDto): Promise<Transacao> {
  const resposta = await fetch(`${BASE_URL}/transacoes`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return tratarResposta<Transacao>(resposta);
}

// --- Endpoint de Totais ---

// Busca os totais gerais (receitas, despesas e saldo por pessoa e geral)
export async function buscarTotais(): Promise<TotaisGeral> {
  const resposta = await fetch(`${BASE_URL}/totais`);
  return tratarResposta<TotaisGeral>(resposta);
}

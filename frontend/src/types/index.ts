// Representa uma pessoa cadastrada no sistema
export interface Pessoa {
  id: number;
  nome: string;
  idade: number;
}

// DTO para criação de pessoa
export interface CriarPessoaDto {
  nome: string;
  idade: number;
}

// Enum para tipo de transação
export enum TipoTransacao {
  Despesa = 0,
  Receita = 1,
}

// Representa uma transação cadastrada no sistema
export interface Transacao {
  id: number;
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  tipoDescricao: string;
  pessoaId: number;
  pessoaNome: string;
}

// DTO para criação de transação
export interface CriarTransacaoDto {
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  pessoaId: number;
}

// Totais por pessoa
export interface PessoaTotais {
  pessoaId: number;
  nome: string;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
}

// Totais gerais do sistema
export interface TotaisGeral {
  pessoas: PessoaTotais[];
  totalGeralReceitas: number;
  totalGeralDespesas: number;
  saldoGeral: number;
}

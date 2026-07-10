namespace backend.DTOs
{
    /// <summary>
    /// DTO que representa os totais financeiros de uma pessoa específica.
    /// Contém o resumo de receitas, despesas e saldo individual.
    /// </summary>
    public class PessoaTotaisDto
    {
        /// <summary>
        /// Identificador único da pessoa.
        /// </summary>
        public int PessoaId { get; set; }

        /// <summary>
        /// Nome da pessoa.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Soma total de todas as receitas da pessoa.
        /// Calculado somando todas as transações do tipo Receita (TipoTransacao = 1).
        /// </summary>
        public decimal TotalReceitas { get; set; }

        /// <summary>
        /// Soma total de todas as despesas da pessoa.
        /// Calculado somando todas as transações do tipo Despesa (TipoTransacao = 0).
        /// </summary>
        public decimal TotalDespesas { get; set; }

        /// <summary>
        /// Saldo da pessoa, calculado como: TotalReceitas - TotalDespesas.
        /// Um saldo positivo indica que a pessoa tem mais receitas do que despesas.
        /// Um saldo negativo indica que a pessoa está gastando mais do que recebe.
        /// </summary>
        public decimal Saldo { get; set; }
    }

    /// <summary>
    /// DTO que representa os totais financeiros gerais do sistema.
    /// Contém o resumo individual de cada pessoa e os totais globais.
    /// Este DTO é retornado pelo endpoint /api/totais.
    /// </summary>
    public class TotaisGeralDto
    {
        /// <summary>
        /// Lista com os totais individuais de cada pessoa cadastrada.
        /// Inclui receitas, despesas e saldo de cada uma.
        /// </summary>
        public List<PessoaTotaisDto> Pessoas { get; set; } = new List<PessoaTotaisDto>();

        /// <summary>
        /// Soma total de todas as receitas de todas as pessoas.
        /// Representa a receita global do sistema de controle de gastos.
        /// </summary>
        public decimal TotalGeralReceitas { get; set; }

        /// <summary>
        /// Soma total de todas as despesas de todas as pessoas.
        /// Representa a despesa global do sistema de controle de gastos.
        /// </summary>
        public decimal TotalGeralDespesas { get; set; }

        /// <summary>
        /// Saldo geral do sistema, calculado como: TotalGeralReceitas - TotalGeralDespesas.
        /// Representa a saúde financeira geral da residência.
        /// </summary>
        public decimal SaldoGeral { get; set; }
    }
}

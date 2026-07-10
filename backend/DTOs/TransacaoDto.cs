using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.DTOs
{
    /// <summary>
    /// DTO para criação de uma nova transação financeira.
    /// Contém os campos necessários para registrar uma receita ou despesa.
    /// </summary>
    public class CriarTransacaoDto
    {
        /// <summary>
        /// Descrição da transação (ex: "Conta de água", "Salário mensal").
        /// Campo obrigatório - não pode ser vazio.
        /// </summary>
        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Valor monetário da transação. Deve ser maior que zero.
        /// Valores negativos ou zero não são permitidos.
        /// </summary>
        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Tipo da transação: Despesa (0) ou Receita (1).
        /// Regra de negócio importante:
        /// - Menores de 18 anos só podem cadastrar Despesas.
        /// - Receitas só podem ser cadastradas por maiores de idade.
        /// </summary>
        [Required(ErrorMessage = "O tipo da transação é obrigatório.")]
        public TipoTransacao Tipo { get; set; }

        /// <summary>
        /// ID da pessoa associada a esta transação.
        /// Deve corresponder a uma pessoa existente no banco de dados.
        /// </summary>
        [Required(ErrorMessage = "O ID da pessoa é obrigatório.")]
        public int PessoaId { get; set; }
    }

    /// <summary>
    /// DTO de resposta ao consultar dados de uma transação.
    /// Inclui informações da pessoa associada para facilitar a exibição.
    /// </summary>
    public class TransacaoResponseDto
    {
        /// <summary>
        /// Identificador único da transação.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descrição da transação.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Valor monetário da transação.
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Tipo da transação como enum (Despesa = 0, Receita = 1).
        /// </summary>
        public TipoTransacao Tipo { get; set; }

        /// <summary>
        /// Descrição textual do tipo da transação: "Despesa" ou "Receita".
        /// Facilita a exibição no frontend sem necessidade de conversão.
        /// </summary>
        public string TipoDescricao { get; set; } = string.Empty;

        /// <summary>
        /// ID da pessoa dona da transação.
        /// </summary>
        public int PessoaId { get; set; }

        /// <summary>
        /// Nome da pessoa dona da transação.
        /// Obtido através da propriedade de navegação do Entity Framework.
        /// </summary>
        public string PessoaNome { get; set; } = string.Empty;
    }
}

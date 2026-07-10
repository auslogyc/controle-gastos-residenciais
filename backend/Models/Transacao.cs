using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    /// <summary>
    /// Enumeração que define os tipos de transação financeira.
    /// Despesa = 0: Representa um gasto/saída de dinheiro.
    /// Receita = 1: Representa uma entrada de dinheiro.
    /// </summary>
    public enum TipoTransacao
    {
        /// <summary>
        /// Representa uma despesa (saída de dinheiro).
        /// </summary>
        Despesa = 0,

        /// <summary>
        /// Representa uma receita (entrada de dinheiro).
        /// Regra de negócio: Menores de 18 anos NÃO podem cadastrar receitas.
        /// </summary>
        Receita = 1
    }

    /// <summary>
    /// Modelo que representa uma transação financeira no sistema.
    /// Uma transação pode ser uma receita ou despesa e está sempre associada a uma pessoa.
    /// </summary>
    public class Transacao
    {
        /// <summary>
        /// Identificador único da transação, gerado automaticamente pelo banco de dados.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Descrição da transação (ex: "Conta de luz", "Salário").
        /// Campo obrigatório.
        /// </summary>
        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Valor monetário da transação. Deve ser maior que zero.
        /// Armazenado como decimal para precisão monetária.
        /// </summary>
        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Tipo da transação: Despesa (0) ou Receita (1).
        /// Utilizado para calcular totais e saldos de cada pessoa.
        /// </summary>
        [Required(ErrorMessage = "O tipo da transação é obrigatório.")]
        public TipoTransacao Tipo { get; set; }

        /// <summary>
        /// Chave estrangeira que referencia a pessoa dona desta transação.
        /// </summary>
        [Required(ErrorMessage = "O ID da pessoa é obrigatório.")]
        public int PessoaId { get; set; }

        /// <summary>
        /// Propriedade de navegação para a pessoa associada a esta transação.
        /// Permite acessar os dados da pessoa diretamente a partir da transação.
        /// </summary>
        [ForeignKey("PessoaId")]
        public Pessoa? Pessoa { get; set; }
    }
}

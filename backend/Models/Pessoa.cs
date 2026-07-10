using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    /// <summary>
    /// Modelo que representa uma pessoa no sistema de controle de gastos residenciais.
    /// Cada pessoa pode ter múltiplas transações (receitas e despesas) associadas.
    /// </summary>
    public class Pessoa
    {
        /// <summary>
        /// Identificador único da pessoa, gerado automaticamente pelo banco de dados.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Nome da pessoa. Campo obrigatório.
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Idade da pessoa. Campo obrigatório e deve ser maior ou igual a zero.
        /// A idade é utilizada para validar regras de negócio:
        /// - Menores de 18 anos só podem cadastrar despesas (não podem cadastrar receitas).
        /// </summary>
        [Required(ErrorMessage = "A idade é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "A idade deve ser maior ou igual a zero.")]
        public int Idade { get; set; }

        /// <summary>
        /// Lista de transações associadas a esta pessoa.
        /// Propriedade de navegação do Entity Framework.
        /// Quando a pessoa é deletada, todas as transações são removidas em cascata.
        /// </summary>
        public List<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }
}

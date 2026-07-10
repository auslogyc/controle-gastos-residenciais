using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    /// <summary>
    /// DTO para criação de uma nova pessoa.
    /// Contém apenas os campos necessários para o cadastro.
    /// O Id é gerado automaticamente pelo banco de dados.
    /// </summary>
    public class CriarPessoaDto
    {
        /// <summary>
        /// Nome da pessoa a ser cadastrada. Campo obrigatório.
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Idade da pessoa a ser cadastrada. Campo obrigatório.
        /// Deve ser maior ou igual a zero.
        /// </summary>
        [Required(ErrorMessage = "A idade é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "A idade deve ser maior ou igual a zero.")]
        public int Idade { get; set; }
    }

    /// <summary>
    /// DTO de resposta ao consultar dados de uma pessoa.
    /// Retorna os dados básicos sem incluir as transações.
    /// </summary>
    public class PessoaResponseDto
    {
        /// <summary>
        /// Identificador único da pessoa.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome da pessoa.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Idade da pessoa.
        /// </summary>
        public int Idade { get; set; }
    }
}

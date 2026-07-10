using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs;

namespace backend.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de pessoas no sistema.
    /// Permite listar, criar e deletar pessoas.
    /// Rota base: /api/pessoas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PessoasController : ControllerBase
    {
        // Contexto do banco de dados injetado via construtor
        private readonly AppDbContext _contexto;

        /// <summary>
        /// Construtor do controller que recebe o contexto do banco de dados
        /// através de injeção de dependência.
        /// </summary>
        /// <param name="contexto">Contexto do banco de dados da aplicação</param>
        public PessoasController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        /// <summary>
        /// GET /api/pessoas
        /// Lista todas as pessoas cadastradas no sistema.
        /// Retorna uma lista de PessoaResponseDto com Id, Nome e Idade.
        /// </summary>
        /// <returns>Lista de todas as pessoas cadastradas</returns>
        [HttpGet]
        public async Task<ActionResult<List<PessoaResponseDto>>> ListarTodas()
        {
            // Busca todas as pessoas no banco de dados
            var pessoas = await _contexto.Pessoas
                .Select(p => new PessoaResponseDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Idade = p.Idade
                })
                .ToListAsync();

            return Ok(pessoas);
        }

        /// <summary>
        /// POST /api/pessoas
        /// Cria uma nova pessoa no sistema.
        /// Validações realizadas:
        /// 1. O nome não pode ser vazio ou nulo.
        /// 2. A idade deve ser maior ou igual a zero.
        /// </summary>
        /// <param name="dto">Dados da pessoa a ser criada</param>
        /// <returns>A pessoa criada com seu Id gerado</returns>
        [HttpPost]
        public async Task<ActionResult<PessoaResponseDto>> Criar([FromBody] CriarPessoaDto dto)
        {
            // ========================================================
            // Validação 1: Verificar se o nome foi informado
            // O nome é obrigatório e não pode ser uma string vazia
            // ========================================================
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                return BadRequest(new { mensagem = "O nome é obrigatório e não pode ser vazio." });
            }

            // ========================================================
            // Validação 2: Verificar se a idade é válida
            // A idade deve ser um número inteiro maior ou igual a zero
            // ========================================================
            if (dto.Idade < 0)
            {
                return BadRequest(new { mensagem = "A idade deve ser maior ou igual a zero." });
            }

            // Cria a entidade Pessoa a partir do DTO recebido
            var pessoa = new Pessoa
            {
                Nome = dto.Nome,
                Idade = dto.Idade
            };

            // Adiciona a nova pessoa ao contexto do banco de dados
            _contexto.Pessoas.Add(pessoa);

            // Salva as alterações no banco de dados (persiste a nova pessoa)
            await _contexto.SaveChangesAsync();

            // Monta o DTO de resposta com os dados da pessoa criada
            var resposta = new PessoaResponseDto
            {
                Id = pessoa.Id,
                Nome = pessoa.Nome,
                Idade = pessoa.Idade
            };

            // Retorna 201 Created com os dados da pessoa criada
            return CreatedAtAction(nameof(ListarTodas), new { id = pessoa.Id }, resposta);
        }

        /// <summary>
        /// DELETE /api/pessoas/{id}
        /// Deleta uma pessoa pelo seu Id.
        /// A deleção é em cascata: quando a pessoa é removida,
        /// todas as suas transações associadas também são deletadas.
        /// Retorna 404 se a pessoa não for encontrada.
        /// </summary>
        /// <param name="id">Id da pessoa a ser deletada</param>
        /// <returns>NoContent (204) se deletada com sucesso, NotFound (404) se não encontrada</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            // Busca a pessoa pelo Id no banco de dados
            var pessoa = await _contexto.Pessoas.FindAsync(id);

            // ========================================================
            // Validação: Verificar se a pessoa existe
            // Se não existir, retorna 404 Not Found com mensagem explicativa
            // ========================================================
            if (pessoa == null)
            {
                return NotFound(new { mensagem = $"Pessoa com Id {id} não foi encontrada." });
            }

            // Remove a pessoa do contexto do banco de dados
            // As transações associadas serão deletadas automaticamente
            // por causa da configuração de deleção em cascata (Cascade Delete)
            _contexto.Pessoas.Remove(pessoa);

            // Salva as alterações no banco de dados
            await _contexto.SaveChangesAsync();

            // Retorna 204 No Content indicando que a deleção foi bem-sucedida
            return NoContent();
        }
    }
}

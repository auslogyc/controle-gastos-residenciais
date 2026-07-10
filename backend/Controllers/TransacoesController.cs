using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs;

namespace backend.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de transações financeiras.
    /// Permite listar e criar transações (receitas e despesas).
    /// Rota base: /api/transacoes
    /// 
    /// Regras de negócio importantes:
    /// 1. Toda transação deve estar associada a uma pessoa existente.
    /// 2. Menores de 18 anos só podem cadastrar despesas (não podem ter receitas).
    /// 3. O valor da transação deve ser maior que zero.
    /// 4. A descrição da transação é obrigatória.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TransacoesController : ControllerBase
    {
        // Contexto do banco de dados injetado via construtor
        private readonly AppDbContext _contexto;

        /// <summary>
        /// Construtor do controller que recebe o contexto do banco de dados
        /// através de injeção de dependência.
        /// </summary>
        /// <param name="contexto">Contexto do banco de dados da aplicação</param>
        public TransacoesController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        /// <summary>
        /// GET /api/transacoes
        /// Lista todas as transações cadastradas no sistema.
        /// Inclui a propriedade de navegação Pessoa para retornar o nome
        /// da pessoa associada a cada transação.
        /// </summary>
        /// <returns>Lista de todas as transações com dados da pessoa associada</returns>
        [HttpGet]
        public async Task<ActionResult<List<TransacaoResponseDto>>> ListarTodas()
        {
            // Busca todas as transações incluindo os dados da pessoa associada
            // O Include carrega a propriedade de navegação Pessoa (eager loading)
            var transacoes = await _contexto.Transacoes
                .Include(t => t.Pessoa)
                .Select(t => new TransacaoResponseDto
                {
                    Id = t.Id,
                    Descricao = t.Descricao,
                    Valor = t.Valor,
                    Tipo = t.Tipo,
                    // Converte o enum para texto legível: "Despesa" ou "Receita"
                    TipoDescricao = t.Tipo == TipoTransacao.Despesa ? "Despesa" : "Receita",
                    PessoaId = t.PessoaId,
                    // Obtém o nome da pessoa através da propriedade de navegação
                    PessoaNome = t.Pessoa != null ? t.Pessoa.Nome : string.Empty
                })
                .ToListAsync();

            return Ok(transacoes);
        }

        /// <summary>
        /// POST /api/transacoes
        /// Cria uma nova transação financeira no sistema.
        /// 
        /// Validações realizadas (em ordem):
        /// 1. Verifica se a descrição não está vazia.
        /// 2. Verifica se o valor é maior que zero.
        /// 3. Verifica se a pessoa (PessoaId) existe no banco de dados.
        /// 4. Verifica a regra de negócio de menores de idade:
        ///    - Se a pessoa tem menos de 18 anos E o tipo é Receita,
        ///      a transação é rejeitada com mensagem explicativa.
        /// </summary>
        /// <param name="dto">Dados da transação a ser criada</param>
        /// <returns>A transação criada com seus dados completos</returns>
        [HttpPost]
        public async Task<ActionResult<TransacaoResponseDto>> Criar([FromBody] CriarTransacaoDto dto)
        {
            // ========================================================
            // Validação 1: Verificar se a descrição foi informada
            // A descrição é obrigatória para identificar a transação
            // ========================================================
            if (string.IsNullOrWhiteSpace(dto.Descricao))
            {
                return BadRequest(new { mensagem = "A descrição é obrigatória e não pode ser vazia." });
            }

            // ========================================================
            // Validação 2: Verificar se o valor é positivo
            // Transações com valor zero ou negativo não são permitidas
            // ========================================================
            if (dto.Valor <= 0)
            {
                return BadRequest(new { mensagem = "O valor deve ser maior que zero." });
            }

            // ========================================================
            // Validação 3: Verificar se a pessoa existe no banco
            // A transação deve estar associada a uma pessoa válida
            // Retorna 400 Bad Request se a pessoa não for encontrada
            // ========================================================
            var pessoa = await _contexto.Pessoas.FindAsync(dto.PessoaId);
            if (pessoa == null)
            {
                return BadRequest(new { mensagem = $"Pessoa com Id {dto.PessoaId} não foi encontrada." });
            }

            // ========================================================
            // Validação 4: Regra de negócio para menores de idade
            // Menores de 18 anos NÃO podem cadastrar receitas.
            // Esta é uma regra de negócio do sistema de controle de gastos
            // residenciais que impede que menores registrem entradas de dinheiro.
            // Apenas despesas são permitidas para pessoas com idade inferior a 18.
            // ========================================================
            if (pessoa.Idade < 18 && dto.Tipo == TipoTransacao.Receita)
            {
                return BadRequest(new { mensagem = "Menores de idade só podem cadastrar despesas." });
            }

            // Cria a entidade Transação a partir do DTO recebido
            var transacao = new Transacao
            {
                Descricao = dto.Descricao,
                Valor = dto.Valor,
                Tipo = dto.Tipo,
                PessoaId = dto.PessoaId
            };

            // Adiciona a nova transação ao contexto do banco de dados
            _contexto.Transacoes.Add(transacao);

            // Salva as alterações no banco de dados (persiste a nova transação)
            await _contexto.SaveChangesAsync();

            // Monta o DTO de resposta com os dados completos da transação criada
            var resposta = new TransacaoResponseDto
            {
                Id = transacao.Id,
                Descricao = transacao.Descricao,
                Valor = transacao.Valor,
                Tipo = transacao.Tipo,
                // Converte o enum para texto legível
                TipoDescricao = transacao.Tipo == TipoTransacao.Despesa ? "Despesa" : "Receita",
                PessoaId = transacao.PessoaId,
                // Usa o nome da pessoa que já foi carregada anteriormente
                PessoaNome = pessoa.Nome
            };

            // Retorna 201 Created com os dados da transação criada
            return CreatedAtAction(nameof(ListarTodas), new { id = transacao.Id }, resposta);
        }
    }
}

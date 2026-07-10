using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs;

namespace backend.Controllers
{
    /// <summary>
    /// Controller responsável por calcular e retornar os totais financeiros.
    /// Fornece um resumo consolidado de receitas, despesas e saldo
    /// tanto individual (por pessoa) quanto geral (todas as pessoas).
    /// Rota base: /api/totais
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TotaisController : ControllerBase
    {
        // Contexto do banco de dados injetado via construtor
        private readonly AppDbContext _contexto;

        /// <summary>
        /// Construtor do controller que recebe o contexto do banco de dados
        /// através de injeção de dependência.
        /// </summary>
        /// <param name="contexto">Contexto do banco de dados da aplicação</param>
        public TotaisController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        /// <summary>
        /// GET /api/totais
        /// Calcula e retorna os totais financeiros do sistema.
        /// 
        /// O retorno inclui:
        /// 1. Lista de todas as pessoas com seus totais individuais:
        ///    - TotalReceitas: soma de todas as transações do tipo Receita
        ///    - TotalDespesas: soma de todas as transações do tipo Despesa
        ///    - Saldo: diferença entre receitas e despesas (Receitas - Despesas)
        /// 
        /// 2. Totais gerais (soma de todas as pessoas):
        ///    - TotalGeralReceitas: soma de todas as receitas do sistema
        ///    - TotalGeralDespesas: soma de todas as despesas do sistema
        ///    - SaldoGeral: diferença entre receitas e despesas gerais
        /// </summary>
        /// <returns>TotaisGeralDto com resumo financeiro completo</returns>
        [HttpGet]
        public async Task<ActionResult<TotaisGeralDto>> ObterTotais()
        {
            // ========================================================
            // Busca todas as pessoas com suas transações associadas
            // O Include carrega as transações de cada pessoa (eager loading)
            // ========================================================
            var pessoas = await _contexto.Pessoas
                .Include(p => p.Transacoes)
                .ToListAsync();

            // ========================================================
            // Calcula os totais individuais de cada pessoa
            // Para cada pessoa, soma separadamente receitas e despesas
            // e calcula o saldo individual (Receitas - Despesas)
            // ========================================================
            var pessoasTotais = pessoas.Select(p => new PessoaTotaisDto
            {
                PessoaId = p.Id,
                Nome = p.Nome,

                // Soma todas as transações do tipo Receita desta pessoa
                // Se não houver receitas, o resultado será 0
                TotalReceitas = p.Transacoes
                    .Where(t => t.Tipo == TipoTransacao.Receita)
                    .Sum(t => t.Valor),

                // Soma todas as transações do tipo Despesa desta pessoa
                // Se não houver despesas, o resultado será 0
                TotalDespesas = p.Transacoes
                    .Where(t => t.Tipo == TipoTransacao.Despesa)
                    .Sum(t => t.Valor),

                // Calcula o saldo: Receitas - Despesas
                // Saldo positivo = pessoa recebe mais do que gasta
                // Saldo negativo = pessoa gasta mais do que recebe
                Saldo = p.Transacoes
                    .Where(t => t.Tipo == TipoTransacao.Receita)
                    .Sum(t => t.Valor)
                    - p.Transacoes
                    .Where(t => t.Tipo == TipoTransacao.Despesa)
                    .Sum(t => t.Valor)
            }).ToList();

            // ========================================================
            // Calcula os totais gerais somando os totais de todas as pessoas
            // Isso fornece uma visão consolidada das finanças da residência
            // ========================================================
            var resultado = new TotaisGeralDto
            {
                Pessoas = pessoasTotais,

                // Soma total de todas as receitas de todas as pessoas
                TotalGeralReceitas = pessoasTotais.Sum(p => p.TotalReceitas),

                // Soma total de todas as despesas de todas as pessoas
                TotalGeralDespesas = pessoasTotais.Sum(p => p.TotalDespesas),

                // Saldo geral: Total de Receitas - Total de Despesas
                SaldoGeral = pessoasTotais.Sum(p => p.TotalReceitas)
                    - pessoasTotais.Sum(p => p.TotalDespesas)
            };

            return Ok(resultado);
        }
    }
}

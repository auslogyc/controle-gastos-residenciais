using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    /// <summary>
    /// Contexto do banco de dados da aplicação.
    /// Responsável por gerenciar as entidades Pessoa e Transação
    /// e configurar o mapeamento objeto-relacional com o SQLite.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Construtor que recebe as opções de configuração do DbContext.
        /// As opções incluem a string de conexão com o banco SQLite.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Tabela de pessoas no banco de dados.
        /// Cada pessoa representa um morador ou participante do controle financeiro.
        /// </summary>
        public DbSet<Pessoa> Pessoas { get; set; } = null!;

        /// <summary>
        /// Tabela de transações no banco de dados.
        /// Cada transação representa uma receita ou despesa de uma pessoa.
        /// </summary>
        public DbSet<Transacao> Transacoes { get; set; } = null!;

        /// <summary>
        /// Configuração do modelo de dados usando Fluent API.
        /// Define relacionamentos, comportamento de deleção em cascata e restrições.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================================================
            // Configuração da entidade Pessoa
            // ========================================================

            modelBuilder.Entity<Pessoa>(entity =>
            {
                // Define o nome da tabela no banco de dados
                entity.ToTable("Pessoas");

                // Configura a chave primária
                entity.HasKey(p => p.Id);

                // Configura o campo Nome como obrigatório
                entity.Property(p => p.Nome)
                    .IsRequired()
                    .HasMaxLength(200);

                // Configura o campo Idade como obrigatório
                entity.Property(p => p.Idade)
                    .IsRequired();
            });

            // ========================================================
            // Configuração da entidade Transação
            // ========================================================

            modelBuilder.Entity<Transacao>(entity =>
            {
                // Define o nome da tabela no banco de dados
                entity.ToTable("Transacoes");

                // Configura a chave primária
                entity.HasKey(t => t.Id);

                // Configura o campo Descrição como obrigatório
                entity.Property(t => t.Descricao)
                    .IsRequired()
                    .HasMaxLength(500);

                // Configura o campo Valor como obrigatório com precisão para valores monetários
                entity.Property(t => t.Valor)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                // Configura o campo Tipo como obrigatório
                entity.Property(t => t.Tipo)
                    .IsRequired();

                // ========================================================
                // Configuração do relacionamento Pessoa -> Transações
                // ========================================================
                // Uma pessoa pode ter muitas transações (1:N)
                // Quando uma pessoa é deletada, todas as suas transações
                // são automaticamente removidas (deleção em cascata).
                // Isso garante integridade referencial no banco de dados.
                entity.HasOne(t => t.Pessoa)
                    .WithMany(p => p.Transacoes)
                    .HasForeignKey(t => t.PessoaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ========================================================
            // Nenhum dado inicial (seed) é configurado.
            // O banco começa vazio e os dados são inseridos via API.
            // ========================================================
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Agenda.Dominio.Reflection;

namespace Agenda.Dominio.Entidades.Acessos
{
    [Table("perfil", Schema = "public")]
    [DebuggerStepThrough]
    public class Perfil
    {
        public Perfil()
        {
            this.SetValuesDefault();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: id", AllowEmptyStrings = false)]
        [Column("id", Order = 1, TypeName = "bigint")]
        public long ID { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idcliente", AllowEmptyStrings = true)]
        [Column("idcliente", Order = 2, TypeName = "bigint")]
        public long IdCliente { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: codigo", AllowEmptyStrings = false)]
        [Column("codigo", Order = 3, TypeName = "smallint")]
        public int Codigo { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(50, ErrorMessage = "tamanho máximo 50 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: idrole", AllowEmptyStrings = true)]
        [Column("idrole", Order = 4, TypeName = "varchar(50)")]
        public string IdRole { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(50, ErrorMessage = "tamanho máximo 50 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: descricao", AllowEmptyStrings = true)]
        [Column("descricao", Order = 5, TypeName = "varchar(50)")]
        public string Descricao { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: status", AllowEmptyStrings = true)]
        [Column("status", Order = 6, TypeName = "tinyint")]
        public int Status { get; set; } = default!;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: horaini", AllowEmptyStrings = true)]
        [Column("horaini", Order = 7, TypeName = "datetime")]
        public DateTime HoraIni { get; set; } = default!;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: horafim", AllowEmptyStrings = true)]
        [Column("horafim", Order = 8, TypeName = "datetime")]
        public DateTime HoraFim { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: semana", AllowEmptyStrings = true)]
        [Column("semana", Order = 9, TypeName = "tinyint")]
        [Range(0, 1, ErrorMessage = "Semana: só é pemitido informar 0(zero) ou 1(um)")]
        public int Semana { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: sabado", AllowEmptyStrings = true)]
        [Column("sabado", Order = 10, TypeName = "tinyint")]
        [Range(0, 1, ErrorMessage = "Sabádo: só é pemitido informar 0(zero) ou 1(um)")]
        public int Sabado { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: domingo", AllowEmptyStrings = true)]
        [Column("domingo", Order = 11, TypeName = "tinyint")]
        [Range(0, 1, ErrorMessage = "Domingo: só é pemitido informar 0(zero) ou 1(um)")]
        public int Domingo { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: mostrarsenha", AllowEmptyStrings = true)]
        [Column("mostrarsenha", Order = 12, TypeName = "tinyint")]
        [Range(0, 1, ErrorMessage = "Mostrar senha agente: só é pemitido informar 0(zero) ou 1(um)")]
        public int MostrarSenha { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: alterarsenha", AllowEmptyStrings = true)]
        [Column("alterarsenha", Order = 13, TypeName = "tinyint")]
        [Range(0, 1, ErrorMessage = "Alterar Senha: só é pemitido informar 0(zero) ou 1(um)")]
        public int AlterarSenha { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: trocarsenha", AllowEmptyStrings = true)]
        [Column("trocarsenha", Order = 14, TypeName = "tinyint")]
        [Range(0, 1, ErrorMessage = "Forçar troca de senha: só é pemitido informar 0(zero) ou 1(um)")]
        public int TrocarSenha { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: permiteexcluir", AllowEmptyStrings = true)]
        [Range(0, 1, ErrorMessage = "Permite excluir: só é pemitido informar 0(zero) ou 1(um)")]
        [Column("permiteexcluir", Order = 15, TypeName = "tinyint")]
        public int PermiteExcluir { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: remuneracao", AllowEmptyStrings = true)]
        [Column("remuneracao", Order = 16, TypeName = "tinyint")]
        [Range(0, 1, ErrorMessage = "Mostrar remuneração: só é pemitido informar 0(zero) ou 1(um)")]
        public int Remuneracao { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: inativar", AllowEmptyStrings = true)]
        [Column("inativar", Order = 17, TypeName = "tinyint")]
        [Range(0, 1, ErrorMessage = "Inativar com 5 dias: só é pemitido informar 0(zero) ou 1(um)")]
        public int Inativar { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: padraoempresa", AllowEmptyStrings = true)]
        [Column("padraoempresa", Order = 18, TypeName = "tinyint")]
        [Range(0, 1, ErrorMessage = "Perfil padrão: só é pemitido informar 0(zero) ou 1(um)")]
        public int PadraoEmpresa { get; set; } = default!;


        [Required(ErrorMessage = "obrigatório informar a propriedade: autoagendamento", AllowEmptyStrings = true)]
        [Column("autoagendamento", Order = 19, TypeName = "tinyint")]
        [Range(0, 1, ErrorMessage = "Gerar agendamento: só é pemitido informar 0(zero) ou 1(um)")]
        public int AutoAgendamento { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(200, ErrorMessage = "tamanho máximo 200 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: controleip", AllowEmptyStrings = true)]
        [Column("controleip", Order = 20, TypeName = "varchar(200)")]
        public string ControleIP { get; set; } = default!;


        [NotMapped]
        [ScaffoldColumn(false)]
        public long RecordsTotal { get; set; } = default!;

        [NotMapped]
        [ScaffoldColumn(false)]
        public string HoraIniTexto { get; set; } = default!;
        [NotMapped]
        [ScaffoldColumn(false)]
        public string HoraFimTexto { get; set; } = default!;
        [NotMapped]
        [ScaffoldColumn(false)]
        public string Nivel { get; set; } = default!;
    }
}

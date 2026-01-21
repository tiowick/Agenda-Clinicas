using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Agenda.Dominio.Reflection;

namespace Agenda.Dominio.Entidades.Acessos
{
    [Table("usuarios", Schema = "acessos")]
    [DebuggerStepThrough]
    public class Usuarios
    {
        public Usuarios()
        {
            this.SetValuesDefault();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: id", AllowEmptyStrings = false)]
        [Column("id", Order = 1, TypeName = "bigint")]
        public long ID { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idcliente", AllowEmptyStrings = false)]
        [Column("idempresa", Order = 2, TypeName = "bigint")]
        public long IdEmpresa { get; set; } = default!;

        [MaxLength(15, ErrorMessage = "tamanho máximo 15 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: telefone", AllowEmptyStrings = true)]
        [Column("telefone", Order = 3, TypeName = "varchar(15)")]
        public string Telefone { get; set; } = default!;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: dtcriacao", AllowEmptyStrings = false)]
        [Column("dtcriacao", Order = 5, TypeName = "datetime")]
        public DateTime DtCriacao { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(15, ErrorMessage = "tamanho máximo 15 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: documento", AllowEmptyStrings = false)]
        [Column("documento", Order = 6, TypeName = "varchar(15)")]
        public string Documento { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(15, ErrorMessage = "tamanho máximo 15 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: matricula", AllowEmptyStrings = false)]
        [Column("matricula", Order = 7, TypeName = "varchar(15)")]
        public string Matricula { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(150, ErrorMessage = "tamanho máximo 150 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: descricao", AllowEmptyStrings = false)]
        [Column("nome", Order = 8, TypeName = "varchar(150)")]
        public string Nome { get; set; } = default!;

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "e-mail informado não é válido ex.:user@example.com")]
        [MaxLength(150, ErrorMessage = "tamanho máximo 150 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: email", AllowEmptyStrings = false)]
        [Column("email", Order = 9, TypeName = "varchar(150)")]
        public string Email { get; set; } = default!;

        [DataType(DataType.Text)]
        [DefaultValue("")]
        [MaxLength(150, ErrorMessage = "tamanho máximo 150 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: cidade", AllowEmptyStrings = false)]
        [Column("cidade", Order = 10, TypeName = "varchar(150)")]
        public string? Cidade { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(2, ErrorMessage = "tamanho máximo 2 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: uf", AllowEmptyStrings = false)]
        [Column("uf", Order = 11, TypeName = "varchar(2)")]
        public string? UF { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(15, ErrorMessage = "tamanho máximo 15 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: logon", AllowEmptyStrings = false)]
        [Column("logon", Order = 12, TypeName = "varchar(15)")]
        public string Logon { get; set; } = default!;

        [Category("Security")]
        [PasswordPropertyText(true)]
        [DataType(DataType.Password)]
        [DefaultValue("padrao123!!!")]
        [MaxLength(15, ErrorMessage = "tamanho máximo 15 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: senha", AllowEmptyStrings = false)]
        [Column("senha", Order = 13, TypeName = "varchar(15)")]
        public string? Senha { get; set; } = default!;

        [DataType(DataType.Text)]
        [DefaultValue("")]
        [MaxLength(50, ErrorMessage = "tamanho máximo 50 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: idclains", AllowEmptyStrings = false)]
        [Column("idclains", Order = 14, TypeName = "varchar(50)")]
        public string? IdClains { get; set; } = default!;

        [DefaultValue(1)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: status", AllowEmptyStrings = false)]
        [Column("status", Order = 15, TypeName = "tinyint")]
        [Range(0, 1, ErrorMessage = "só é pemitido informar 0(zero) ou 1(um)")]
        public int Status { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idcampanha", AllowEmptyStrings = false)]
        [Column("idcampanha", Order = 16, TypeName = "bigint")]
        public long IdCampanha { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idusuario", AllowEmptyStrings = false)]
        [Column("idusuario", Order = 17, TypeName = "bigint")]
        public long IdUsuario { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idusuario", AllowEmptyStrings = false)]
        [Column("idvendedor", Order = 18, TypeName = "bigint")]
        public long IdVendedor { get; set; } = default!;



        //[Required(ErrorMessage = "obrigatório informar a propriedade: chavepix", AllowEmptyStrings = false)]
        //[Column("chavepix", Order = 18, TypeName = "varchar(max)")]
        //public string? ChavePix { get; set; } = default!; // 14/01/2026


        [NotMapped]
        [ScaffoldColumn(false)]
        public int StatusLogin { get; set; } = default!;


        [NotMapped]
        [ScaffoldColumn(false)]
        public long RecordsTotal { get; set; } = default!;

        [NotMapped]
        [ScaffoldColumn(false)]
        public string? DtCriacaoTXT { get; set; } = default!;
        [NotMapped]
        [ScaffoldColumn(false)]
        public string? Promotora { get; set; } = default!;

        [NotMapped]
        [ScaffoldColumn(false)]
        public string? GrupoComissao { get; set; } = default!;

    }
}

using Agenda.Dominio.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Entidades.Agenda
{
    [DebuggerStepThrough]
    [Table("agenda", Schema = "public")]
    public class Calendario
    {
        public Calendario()
        {
            this.SetValuesDefault();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: id", AllowEmptyStrings = false)]
        [Column("id", Order = 1, TypeName = "bigint")]
        public int ID { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idvendedor", AllowEmptyStrings = false)]
        [Column("idvendedor", Order = 2, TypeName = "int")]
        public long? IdVendedor { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idempresa", AllowEmptyStrings = false)]
        [Column("idempresa", Order = 3, TypeName = "int")]
        public long? IdEmpresa { get; set; }

        [Required(ErrorMessage = "obrigatório informar a propriedade: idusuario", AllowEmptyStrings = false)]
        [Column("idusuario", Order = 4, TypeName = "integer")]
        public long? IdUsuario { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idlaudo", AllowEmptyStrings = false)]
        [Column("idlaudo", Order = 5, TypeName = "integer")]
        public long? IdLaudo { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idsetor", AllowEmptyStrings = false)]
        [Column("idsetor", Order = 5, TypeName = "integer")]
        public long? IdSetor { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idresponsavel", AllowEmptyStrings = false)]
        [Column("idresponsavel", Order = 5, TypeName = "integer")]
        public long? IdResponsavel { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idsituacao", AllowEmptyStrings = false)]
        [Column("idsituacao", Order = 5, TypeName = "integer")]
        public long? IdSituacao { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idSolicitacao", AllowEmptyStrings = false)]
        [Column("idSolicitacao", Order = 5, TypeName = "integer")]
        public long? IdSolicitacao { get; set; } = default!;

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: descricao", AllowEmptyStrings = true)]
        [Column("descricao", Order = 6, TypeName = "text")]
        public string Descricao { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(255, ErrorMessage = "tamanho máximo 255 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: solicitante", AllowEmptyStrings = true)]
        [Column("solicitante", Order = 7, TypeName = "varchar(255)")]
        public string? Solicitante { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: privado", AllowEmptyStrings = false)]
        [Column("privado", Order = 8, TypeName = "bool")]
        public bool? Privativa { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessage = "tamanho máximo 100 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: responsavel", AllowEmptyStrings = false)]
        [Column("responsavel", Order = 9, TypeName = "varchar(100)")]
        public string? Responsavel { get; set; } = default!;

        [EmailAddress]
        [MaxLength(255, ErrorMessage = "tamanho máximo 255 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: contato", AllowEmptyStrings = false)]
        [Column("contato", Order = 10, TypeName = "varchar(255)")]
        public string? Contato { get; set; } = default!;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: datacriacao", AllowEmptyStrings = false)]
        [Column("datacriacao", Order = 11, TypeName = "timestamp without time zone")]
        public DateTime DataHora { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(60, ErrorMessage = "tamanho máximo 60 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: local", AllowEmptyStrings = false)]
        [Column("local", Order = 12, TypeName = "varchar(60)")]
        public string? Local { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessage = "tamanho máximo 100 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: situacao", AllowEmptyStrings = false)]
        [Column("situacao", Order = 13, TypeName = "varchar(100)")]
        public string? Situacao { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: status", AllowEmptyStrings = false)]
        [Column("status", Order = 14, TypeName = "bool")]
        public bool? Status { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessage = "tamanho máximo 100 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: situacao", AllowEmptyStrings = false)]
        [Column("observacao", Order = 15, TypeName = "text")]
        public string? Observacao { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: historico", AllowEmptyStrings = false)]
        [Column("historico", Order = 16, TypeName = "int")]
        public int? Historico { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessage = "tamanho máximo 100 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: setor", AllowEmptyStrings = false)]
        [Column("setor", Order = 17, TypeName = "varchar(100)")]
        public string? Setor { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(255, ErrorMessage = "tamanho máximo 255 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: objetivo", AllowEmptyStrings = false)]
        [Column("objetivo", Order = 18, TypeName = "varchar(255)")]
        public string? Objetivo { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessage = "tamanho máximo 100 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: objetivo", AllowEmptyStrings = false)]
        [Column("tipo_solicitacao", Order = 19, TypeName = "varchar(100)")]
        public string? TipoSolicitacao { get; set; } = default!;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: datahoraini", AllowEmptyStrings = false)]
        [Column("datahoraini", Order = 20, TypeName = "timestamp without time zone")]
        public DateTime? DataHoraIni { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(255, ErrorMessage = "tamanho máximo 255 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: localizacaoini", AllowEmptyStrings = false)]
        [Column("localizacaoini", Order = 21, TypeName = "varchar(255)")]
        public string? LocalizacaoIni { get; set; } = default!;

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: datahorafim", AllowEmptyStrings = false)]
        [Column("datahorafim", Order = 22, TypeName = "time without time zone")]
        public DateTime? datahorafim { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(255, ErrorMessage = "tamanho máximo 255 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: localizacaofim", AllowEmptyStrings = false)]
        [Column("localizacaofim", Order = 23, TypeName = "varchar(255)")]
        public string? LocalizacaoFim { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(255, ErrorMessage = "tamanho máximo 255 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: laudo", AllowEmptyStrings = false)]
        [Column("laudo", Order = 24, TypeName = "text")]
        public string? Laudo { get; set; } = default!;

        [DefaultValue(1)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: status", AllowEmptyStrings = false)]
        [Column("status", Order = 25, TypeName = "tinyint")]
        [Range(0, 1, ErrorMessage = "só é pemitido informar 0(zero) ou 1(um)")]
        public int? status { get; set; } = default!;

        [NotMapped]
        [ScaffoldColumn(false)]
        public long RecordsTotal { get; set; } = default!;

    }
}

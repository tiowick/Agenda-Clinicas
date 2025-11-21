using Agenda.Dominio.Reflection;
using System;
using System.Collections.Generic;
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
        [Required(ErrorMessage = "obrigatório informar a propriedade: id", AllowEmptyStrings = false)]
        [Column("id", Order = 1, TypeName = "int")]
        public int ID { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: ag_vendedorid", AllowEmptyStrings = false)]
        [Column("ag_vendedorid", Order = 2, TypeName = "int")]
        public int AgVendedorId { get; set; } = default!;

        [Column("idempresa", Order = 3, TypeName = "int")]
        public int? EmpresaId { get; set; }

        // --- ADICIONE ESTA NOVA PROPRIEDADE AQUI ---
        [Column("idusuario", Order = 26, TypeName = "integer")]
        public int? IdUsuario { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: descricao_agenda", AllowEmptyStrings = true)]
        [Column("descricao_agenda", Order = 4, TypeName = "text")]
        public string Descricao { get; set; } = default!;

        [Column("ccusto_id", Order = 5, TypeName = "int")]
        public int? CCusto_Id { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(255, ErrorMessage = "tamanho máximo 255 caracteres")]
        [Column("solicitante_agenda", Order = 6, TypeName = "varchar(255)")]
        public string? Solicitante { get; set; }

        [Column("privado", Order = 7, TypeName = "bool")]
        public bool? Privativa { get; set; } = false;

        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessage = "tamanho máximo 100 caracteres")]
        [Column("responsavel", Order = 8, TypeName = "varchar(100)")]
        public string? Responsavel { get; set; }

        [EmailAddress]
        [MaxLength(255, ErrorMessage = "tamanho máximo 255 caracteres")]
        [Column("contato", Order = 9, TypeName = "varchar(255)")]
        public string? Contato { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: data_hora", AllowEmptyStrings = false)]
        [Column("data_hora", Order = 10, TypeName = "timestamp without time zone")]
        public DateTime DataHora { get; set; }

        [DataType(DataType.Text)]
        [Column("local", Order = 11, TypeName = "varchar(255)")]
        public string? Local { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessage = "tamanho máximo 100 caracteres")]
        [Column("situacao", Order = 12, TypeName = "varchar(100)")]
        public string? Situacao { get; set; }

        [Column("status", Order = 13, TypeName = "bool")]
        public bool? Status { get; set; } = false;

        [DataType(DataType.Text)]
        [Column("observacao", Order = 14, TypeName = "text")]
        public string? Observacao { get; set; }

        [Column("historico", Order = 15, TypeName = "int")]
        public int? Historico { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessage = "tamanho máximo 100 caracteres")]
        [Column("setor", Order = 16, TypeName = "varchar(100)")]
        public string? Setor { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(255, ErrorMessage = "tamanho máximo 255 caracteres")]
        [Column("objetivo", Order = 17, TypeName = "varchar(255)")]
        public string? Objetivo { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(255, ErrorMessage = "tamanho máximo 255 caracteres")]
        [Column("setor_desc", Order = 18, TypeName = "varchar(255)")]
        public string? SetorDescricao { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessage = "tamanho máximo 100 caracteres")]
        [Column("tipo_solicitacao", Order = 19, TypeName = "varchar(100)")]
        public string? TipoSolicitacao { get; set; }

        [DataType(DataType.DateTime)]
        [Column("ag_datahoraini", Order = 20, TypeName = "timestamp without time zone")]
        public DateTime? AgDatahoraIni { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(255, ErrorMessage = "tamanho máximo 255 caracteres")]
        [Column("ag_localizacaoini", Order = 21, TypeName = "varchar(255)")]
        public string? AgLocalizacaoIni { get; set; }

        [DataType(DataType.Time)]
        [Column("ag_datahorafim", Order = 22, TypeName = "time without time zone")]
        public TimeSpan? AgDatahorafim { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(255, ErrorMessage = "tamanho máximo 255 caracteres")]
        [Column("ag_localizacaofim", Order = 23, TypeName = "varchar(255)")]
        public string? AgLocalizacaoFim { get; set; }

        [DataType(DataType.Text)]
        [Column("ag_laudo", Order = 24, TypeName = "text")]
        public string? AgLaudo { get; set; }

        [Column("ag_codstatus", Order = 25, TypeName = "int")]
        public int? AgCodStatus { get; set; }

        [NotMapped]
        [ScaffoldColumn(false)]
        public long RecordsTotal { get; set; } = default!;

    }
}

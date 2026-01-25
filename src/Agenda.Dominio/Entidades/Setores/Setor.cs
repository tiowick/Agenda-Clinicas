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

namespace Agenda.Dominio.Entidades.Setores
{
    [DebuggerStepThrough]
    [Table("setor", Schema = "public")]
    public class Setor
    {
        public Setor()
        {
            this.SetValuesDefault();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: id", AllowEmptyStrings = false)]
        [Column("id", Order = 1, TypeName = "bigint")]
        public int ID { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idempresa", AllowEmptyStrings = false)]
        [Column("idempresa", Order = 2, TypeName = "int")]
        public int IdEmpresa { get; set; }

        [Required(ErrorMessage = "obrigatório informar a propriedade: idusuario", AllowEmptyStrings = false)]
        [Column("idusuario", Order = 3, TypeName = "integer")]
        public int IdUsuario { get; set; } = default!;

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: descricao", AllowEmptyStrings = true)]
        [Column("descricao", Order = 4, TypeName = "text")]
        public string Descricao { get; set; } = default!;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: datacriacao", AllowEmptyStrings = false)]
        [Column("datacriacao", Order = 5, TypeName = "timestamp without time zone")]
        public DateTime DtCriacao { get; set; } = default!;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: datacriacao", AllowEmptyStrings = false)]
        [Column("dataconfirmacao", Order = 6, TypeName = "timestamp without time zone")]
        public DateTime DtConfirmacao { get; set; } = default!;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: datacriacao", AllowEmptyStrings = false)]
        [Column("dataatualizacao", Order = 7, TypeName = "timestamp without time zone")]
        public DateTime DtAtualizacao { get; set; } = default!;

        [DefaultValue(1)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: status", AllowEmptyStrings = false)]
        [Column("status", Order = 8, TypeName = "tinyint")]
        [Range(0, 1, ErrorMessage = "só é pemitido informar 0(zero) ou 1(um)")]
        public int? status { get; set; } = default!;

        [NotMapped]
        [ScaffoldColumn(false)]
        public long RecordsTotal { get; set; } = default!;
    }
}

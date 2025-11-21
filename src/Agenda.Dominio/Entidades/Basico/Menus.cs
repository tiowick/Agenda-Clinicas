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

namespace Agenda.Dominio.Entidades.Basico
{
    [Table("menus", Schema = "public")]
    [DebuggerStepThrough]
    public class Menus
    {
        public Menus()
        {
            this.SetValuesDefault();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: id", AllowEmptyStrings = false)]
        [Column("id", Order = 1, TypeName = "bigint")]
        public long ID { get; set; } = default!;

        [DataType(DataType.Text)]
        [MaxLength(45, ErrorMessage = "tamanho máximo 45 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: descricao", AllowEmptyStrings = false)]
        [Column("descricao", Order = 2, TypeName = "varchar(45)")]
        public string Descricao { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: codigo", AllowEmptyStrings = false)]
        [Column("codigo", Order = 3, TypeName = "int")]
        public int Codigo { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: codigopai", AllowEmptyStrings = true)]
        [Column("codigopai", Order = 4, TypeName = "int")]
        public int CodigoPai { get; set; } = default!;

        [DefaultValue(0)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: ordem", AllowEmptyStrings = false)]
        [Column("ordem", Order = 5, TypeName = "int")]
        public int Ordem { get; set; } = default!;

        [DataType(DataType.Text)]
        [DefaultValue("")]
        [MaxLength(30, ErrorMessage = "tamanho máximo 30 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: area", AllowEmptyStrings = false)]
        [Column("area", Order = 6, TypeName = "varchar(30)")]
        public string Area { get; set; } = default!;

        [DataType(DataType.Text)]
        [DefaultValue("")]
        [MaxLength(30, ErrorMessage = "tamanho máximo 30 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: controller", AllowEmptyStrings = false)]
        [Column("controller", Order = 7, TypeName = "varchar(30)")]
        public string Controller { get; set; } = default!;

        [DataType(DataType.Text)]
        [DefaultValue("")]
        [MaxLength(50, ErrorMessage = "tamanho máximo 50 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: metodo", AllowEmptyStrings = false)]
        [Column("metodo", Order = 8, TypeName = "varchar(50)")]
        public string Metodo { get; set; } = default!;

        [DataType(DataType.Text)]
        [DefaultValue("")]
        [MaxLength(30, ErrorMessage = "tamanho máximo 30 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: icone", AllowEmptyStrings = false)]
        [Column("icone", Order = 9, TypeName = "varchar(30)")]
        public string Icone { get; set; } = default!;

        [DataType(DataType.Text)]
        [DefaultValue("")]
        [MaxLength(50, ErrorMessage = "tamanho máximo 50 caracteres")]
        [Required(ErrorMessage = "obrigatório informar a propriedade: idrole", AllowEmptyStrings = false)]
        [Column("idrole", Order = 10, TypeName = "varchar(50)")]
        public string IdRole { get; set; } = default!;

        [DefaultValue(1)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: status", AllowEmptyStrings = false)]
        [Column("status", Order = 11, TypeName = "tinyint")]
        public int Status { get; set; } = default!;

        [NotMapped]
        [ScaffoldColumn(false)]
        public long RecordsTotal { get; set; } = default!;

    }
}

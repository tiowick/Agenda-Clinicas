using Agenda.Dominio.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Entidades.Acessos
{
    [Table("perfilmenus", Schema = "public")]
    [DebuggerStepThrough]
    public class PerfilMenus
    {
        public PerfilMenus()
        {
            this.SetValuesDefault();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: id", AllowEmptyStrings = false)]
        [Column("id", Order = 1, TypeName = "bigint")]
        public long ID { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idcliente", AllowEmptyStrings = false)]
        [Column("idcliente", Order = 2, TypeName = "bigint")]
        public long IdCliente { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idmenu", AllowEmptyStrings = false)]
        [Column("idmenu", Order = 3, TypeName = "bigint")]
        public long IdMenu { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: idperfil", AllowEmptyStrings = false)]
        [Column("idperfil", Order = 4, TypeName = "bigint")]
        public long IdPerfil { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: status", AllowEmptyStrings = false)]
        [Column("status", Order = 5, TypeName = "int")]
        public int Status { get; set; } = default!;

        [NotMapped]
        [ScaffoldColumn(false)]
        public long RecordsTotal { get; set; } = default!;

        [NotMapped]
        [ScaffoldColumn(false)]
        public string Descricao { get; set; } = default!;
    }
}

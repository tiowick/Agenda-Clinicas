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

namespace Agenda.Aplicacao.Entidades.Produtos
{
    [DebuggerStepThrough]
    [Table("produtos", Schema = "public")]
    public class Produto
    {
        public Produto()
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
        public long? IdEmpresa { get; set; }

        [Required(ErrorMessage = "obrigatório informar a propriedade: idusuario", AllowEmptyStrings = false)]
        [Column("idusuario", Order = 3, TypeName = "integer")]
        public long? IdUsuario { get; set; } = default!;

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: descricao", AllowEmptyStrings = true)]
        [Column("descricao", Order = 4, TypeName = "text")]
        public string Descricao { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: codigo", AllowEmptyStrings = false)]
        [Column("codigo", Order = 5, TypeName = "integer")]
        public long? Codigo { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: tipo", AllowEmptyStrings = false)]
        [Column("tipo", Order = 6, TypeName = "integer")]
        public long? Tipo { get; set; } = default!;

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: unidade", AllowEmptyStrings = true)]
        [Column("unidade", Order = 7, TypeName = "text")]
        public string Unidade { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: valorcusto", AllowEmptyStrings = false)]
        [Column("valorcusto", Order = 8, TypeName = "integer")]
        public long? ValorCusto { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: valorvenda", AllowEmptyStrings = false)]
        [Column("valorvenda", Order = 9, TypeName = "integer")]
        public long? ValorVenda { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: estoqueatual", AllowEmptyStrings = false)]
        [Column("estoqueatual", Order = 10, TypeName = "integer")]
        public long? EstoqueAtual { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: estoqueminimo", AllowEmptyStrings = false)]
        [Column("estoqueminimo", Order = 11, TypeName = "integer")]
        public long? EstoqueMinimo { get; set; } = default!;

        [Required(ErrorMessage = "obrigatório informar a propriedade: controlaestoque", AllowEmptyStrings = false)]
        [Column("controlaestoque", Order = 12, TypeName = "integer")]
        [Range(0, 1, ErrorMessage = "só é pemitido informar 0(zero) ou 1(um)")]
        public long? ControlaEstoque { get; set; } = default!;

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: observacao", AllowEmptyStrings = true)]
        [Column("observacao", Order = 13, TypeName = "text")]
        public string Observacao { get; set; } = default!;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: datacriacao", AllowEmptyStrings = false)]
        [Column("datacriacao", Order = 14, TypeName = "timestamp without time zone")]
        public DateTime DtCriacao { get; set; } = default!;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: dataatualizacao", AllowEmptyStrings = false)]
        [Column("dataatualizacao", Order = 15, TypeName = "timestamp without time zone")]
        public DateTime DtAtualizacao { get; set; } = default!;

        [DefaultValue(1)]
        [Required(ErrorMessage = "obrigatório informar a propriedade: status", AllowEmptyStrings = false)]
        [Column("status", Order = 16, TypeName = "tinyint")]
        [Range(0, 1, ErrorMessage = "só é pemitido informar 0(zero) ou 1(um)")]
        public int? status { get; set; } = default!;

        [NotMapped]
        [ScaffoldColumn(false)]
        public long RecordsTotal { get; set; } = default!;
    }
}

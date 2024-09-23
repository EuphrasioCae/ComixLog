using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace ComixLog.Models
{
    public class Container
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public decimal Preco { get; set; }
        public int CapacidadeTotal { get; set; } 
        public int CapacidadeAtual {  get; set; }
        public bool Resfriado { get; set; } 
        public StatusContainer Status { get; set; }
        public string DataDeEmbarque { get; set; } = string.Empty;
    }
    public enum StatusContainer
    {
        Transporte,
        Lotado,
        Vazio,
        Entregue
    }
   

}

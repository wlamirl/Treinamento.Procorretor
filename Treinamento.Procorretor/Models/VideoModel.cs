using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Treinamento.Procorretor.Models
{
    public class VideoModel
    {
        [Key]
        [DisplayName("Id")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Informar o Título do vídeo")]
        [StringLength(80, ErrorMessage = "O Título deve conter até 80 caracteres!")]
        [MinLength(5, ErrorMessage = "O Título deve conter pelo menos 5 caracteres!")]
        [DisplayName("Título")]
        public string? Title { get; set; }
        [DisplayName("Miniatura")]
        public string? Thumbnail { get; set; }
        [DisplayName("Descrição")]
        public string? Description { get; set; }
        public string? Tag { get; set; }
        public string? CategoryId { get; set; }
        public string? UrlVideo { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.ViewModels.Improvement
{
    public class ImprovementViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Description { get; set; } = null!;
    }
}

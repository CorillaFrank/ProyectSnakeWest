namespace Proyect_Snake_West.Models
{
    public class UsuarioConRolViewModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? RolSeleccionado { get; set; }
        public List<string> RolesDisponibles { get; set; } = new();
    }
}

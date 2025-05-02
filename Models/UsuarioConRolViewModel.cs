namespace Proyect_Snake_West.Models
{
    public class UsuarioConRolViewModel
    {
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string RolSeleccionado { get; set; } = string.Empty;
    public List<string> RolesDisponibles { get; set; } = new();
    }
}


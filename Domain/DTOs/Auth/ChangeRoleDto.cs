namespace Domain.DTOs.Auth;

public class ChangeRoleDto
{
    public string UserId { get; set; }
    public string NewRole { get; set; }
}
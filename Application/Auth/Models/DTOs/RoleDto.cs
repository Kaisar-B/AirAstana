using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Models.DTOs;
public class RoleDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
}
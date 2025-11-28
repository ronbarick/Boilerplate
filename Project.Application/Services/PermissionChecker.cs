using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Core.Interfaces;
using Project.Infrastructure.Data;

namespace Project.Application.Services;

public class PermissionChecker : IPermissionChecker
{
    private readonly AppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public PermissionChecker(AppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<bool> IsGrantedAsync(ClaimsPrincipal user, string permissionName)
    {
        var userId = _currentUser.Id;
        if (!userId.HasValue)
        {
            return false;
        }

        // 1. Check User Permissions (Override)
        var userPermission = await _context.UserPermissions
            
            .FirstOrDefaultAsync(p => p.UserId == userId && p.PermissionName == permissionName);

        if (userPermission != null)
        {
            return userPermission.IsGranted;
        }

        // 2. Check Role Permissions
        var userRoles = await _context.UserRoles
            
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .ToListAsync();

        if (userRoles.Any())
        {
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

            var rolePermissions = await _context.RolePermissions
                
                .Where(p => roleIds.Contains(p.RoleId) && p.PermissionName == permissionName)
                .ToListAsync();

            if (rolePermissions.Any(p => p.IsGranted))
            {
                return true;
            }
        }

        // 3. Check Hierarchical Permissions (e.g., "Rooms" covers "Rooms.Create")
        if (permissionName.Contains('.'))
        {


            // Check user parent permission
            var userParentPermission = await _context.UserPermissions
                
                .FirstOrDefaultAsync(p => p.UserId == userId && p.PermissionName == permissionName);

            if (userParentPermission != null && userParentPermission.IsGranted)
            {
                return true;
            }

            // Check role parent permission
            if (userRoles.Any())
            {
                var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

                var roleParentPermissions = await _context.RolePermissions
                    
                    .Where(p => roleIds.Contains(p.RoleId) && p.PermissionName == permissionName)
                    .ToListAsync();

                if (roleParentPermissions.Any(p => p.IsGranted))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public async Task<bool> IsGrantedAsync(string permissionName)
    {
        // Use the current user from ICurrentUser
        var userId = _currentUser.Id;
        if (!userId.HasValue)
        {
            return false;
        }

        // 1. Check User Permissions (Override)
        var userPermission = await _context.UserPermissions
            
            .FirstOrDefaultAsync(p => p.UserId == userId && p.PermissionName == permissionName);

        if (userPermission != null)
        {
            return userPermission.IsGranted;
        }

        // 2. Check Role Permissions
        var userRoles = await _context.UserRoles
            
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .ToListAsync();

        if (userRoles.Any())
        {
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

            var rolePermissions = await _context.RolePermissions
                
                .Where(p => roleIds.Contains(p.RoleId) && p.PermissionName == permissionName)
                .ToListAsync();

            if (rolePermissions.Any(p => p.IsGranted))
            {
                return true;
            }
        }

        // 3. Check Hierarchical Permissions (e.g., "Students" covers "Students.Create")
        if (permissionName.Contains('.'))
        {
            var parts = permissionName.Split('.');
            var parentPermission = parts[0];

            // Check user parent permission
            var userParentPermission = await _context.UserPermissions
                
                .FirstOrDefaultAsync(p => p.UserId == userId && p.PermissionName == parentPermission);

            if (userParentPermission != null && userParentPermission.IsGranted)
            {
                return true;
            }

            // Check role parent permission
            if (userRoles.Any())
            {
                var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

                var roleParentPermissions = await _context.RolePermissions
                    
                    .Where(p => roleIds.Contains(p.RoleId) && p.PermissionName == parentPermission)
                    .ToListAsync();

                if (roleParentPermissions.Any(p => p.IsGranted))
                {
                    return true;
                }
            }
        }

        return false;
    }
}

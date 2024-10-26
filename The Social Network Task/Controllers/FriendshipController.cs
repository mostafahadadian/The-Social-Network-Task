using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using The_Social_Network_Task.DAL;
using The_Social_Network_Task.Entities;
using The_Social_Network_Task.Framework;

[Authorize]
public class FriendshipController : Controller
{
    private readonly ApplicationContext _context;

    public FriendshipController(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> RequestFriendship(int friendId)
    {

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var existingFriendship = await _context.Friendships
        .FirstOrDefaultAsync(f => (f.UserId == int.Parse(userId) && f.FriendId == friendId)
         || (f.UserId == friendId && f.FriendId == int.Parse(userId)));

        if (existingFriendship != null)
        {
            TempData["Error"] = existingFriendship.Status == FriendshipStatus.Pending
                ? "You have already sent a friendship request."
                : "You are already friends with this user.";
            return RedirectToAction("Index", "User");
        }


        var friendship = new Friendship
        {
            UserId = int.Parse(userId),
            FriendId = friendId,
            Status = FriendshipStatus.Pending
        };

        _context.Friendships.Add(friendship);
        await _context.SaveChangesAsync();
        return RedirectToAction("SentFriendRequests");
    }

    public async Task<IActionResult> AcceptRequest(int Id)
    {
        var friendship = await _context.Friendships.FindAsync(Id);
        if (friendship == null) return NotFound();

        friendship.Status = FriendshipStatus.Accepted;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "User");
    }

    [HttpPost]
    public async Task<IActionResult> RejectRequest(int friendId)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var friendship = await _context.Friendships.FirstOrDefaultAsync(f => f.FriendId == currentUserId && f.UserId == friendId && f.Status == FriendshipStatus.Pending);

        if (friendship != null)
        {
            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Friend request rejected.";
        }
        else
        {
            TempData["Error"] = "Friend request not found.";
        }

        return RedirectToAction("Index", "User");
    }


    public async Task<IActionResult> Unfriend(int Id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendId == Id) || (f.UserId == Id && f.FriendId == userId));

        if (friendship != null)
        {
            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "User");
    }

    public IActionResult Requests()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var pendingRequests = _context.Friendships
            .Where(f => f.FriendId == userId && f.Status == FriendshipStatus.Pending)
            .Include(f => f.User)
            .ToList();

        return View(pendingRequests);
    }


    public async Task<IActionResult> FriendsList()
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var friends = await _context.Friendships
                .Where(f => f.Status == FriendshipStatus.Accepted && (f.UserId == currentUserId || f.FriendId == currentUserId))
                .Select(f => f.UserId == currentUserId ? f.Friend : f.User)
                .ToListAsync();

        return View(friends);
    }

    public async Task<IActionResult> SentFriendRequests()
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var requests = await _context.Friendships
            .Where(f => f.UserId == currentUserId && f.Status == FriendshipStatus.Pending)
            .ToListAsync();

        return View(requests);
    }

    [HttpPost]
    public async Task<IActionResult> CancelRequest(int friendId)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => f.UserId == currentUserId && f.FriendId == friendId && f.Status == FriendshipStatus.Pending);

        if (friendship != null)
        {
            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();

            TempData["Message"] = "request canceled.";
        }
        else
        {
            TempData["Error"] = "request not found.";
        }

        return RedirectToAction("SentFriendRequests");
    }

}
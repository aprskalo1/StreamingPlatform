using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RWAProject.Middleware;
using RWAProject.Models;
using PagedList;

namespace RWAProject.Controllers
{
    [TypeFilter(typeof(AuthFilter))]
    public class VideosController : Controller
    {
        private readonly RwaMoviesContext _context;

        public VideosController(RwaMoviesContext context)
        {
            _context = context;
        }

        // GET: Videos
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.currnetSort = sortOrder;    
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["GenreSortParm"] = sortOrder == "Genre" ? "genre_desc" : "Genre"; // New line for genre sorting

            IQueryable<Video> videosQuery = _context.Videos.Include(v => v.Genre).Include(v => v.Image);

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                videosQuery = videosQuery.Where(v => v.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    videosQuery = videosQuery.OrderByDescending(v => v.Name);
                    break;
                case "Genre": // Sorting by genre ascending
                    videosQuery = videosQuery.OrderBy(v => v.Genre.Name);
                    break;
                case "genre_desc": // Sorting by genre descending
                    videosQuery = videosQuery.OrderByDescending(v => v.Genre.Name);
                    break;
                default:
                    videosQuery = videosQuery.OrderBy(v => v.Name);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            var videos = await videosQuery.ToListAsync();
            return View(videos.ToPagedList(pageNumber, pageSize));
        }


        // GET: Videos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Videos == null)
            {
                return NotFound();
            }

            var video = await _context.Videos
                .Include(v => v.Genre)
                .Include(v => v.Image)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (video == null)
            {
                return NotFound();
            }

            return View(video);
        }

        // GET: Videos/Create
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Id");
            ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id");
            return View();
        }

        // POST: Videos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreatedAt,Name,Description,GenreId,TotalSeconds,StreamingUrl,ImageId")] Video video)
        {
            ModelState.Remove("Genre");
            if (ModelState.IsValid)
            {
                _context.Add(video);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Id", video.GenreId);
            ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id", video.ImageId);
            return View(video);
        }

        // GET: Videos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Videos == null)
            {
                return NotFound();
            }

            var video = await _context.Videos.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Id", video.GenreId);
            ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id", video.ImageId);
            return View(video);
        }

        // POST: Videos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CreatedAt,Name,Description,GenreId,TotalSeconds,StreamingUrl,ImageId")] Video video)
        {
            if (id != video.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Genre");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(video);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideoExists(video.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Id", video.GenreId);
            ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id", video.ImageId);
            return View(video);
        }

        // GET: Videos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Videos == null)
            {
                return NotFound();
            }

            var video = await _context.Videos
                .Include(v => v.Genre)
                .Include(v => v.Image)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (video == null)
            {
                return NotFound();
            }

            return View(video);
        }

        // POST: Videos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Videos == null)
            {
                return Problem("Entity set 'RwaMoviesContext.Videos'  is null.");
            }
            var video = await _context.Videos.FindAsync(id);
            if (video != null)
            {
                _context.Videos.Remove(video);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VideoExists(int id)
        {
          return (_context.Videos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

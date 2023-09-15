using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RWAProject.Models;
using RWAProjectApi.DTOs;

namespace RWAProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly RwaMoviesContext _context;
        private readonly IMapper _mapper;

        public VideosController(RwaMoviesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Videos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Video>>> GetVideos()
        {
            return await _context.Videos.ToListAsync();
        }

        [HttpGet("action")]
        public async Task<ActionResult<IEnumerable<Video>>> SearchVideos(
            string searchString,
            string sortBy = "id",
            string sortOrder = "asc",
            int pageNumber = 1,
            int pageSize = 10)
        {
            IQueryable<Video> videosQuery = _context.Videos;

            if (!String.IsNullOrEmpty(searchString))
            {
                videosQuery = videosQuery.Where(v => v.Name.Contains(searchString));
            }

            if (!new[] { "id", "name", "totaltime" }.Contains(sortBy.ToLower()))
            {
                sortBy = "id";
            }

            switch (sortBy.ToLower())
            {
                case "name":
                    videosQuery = sortOrder.ToLower() == "desc"
                        ? videosQuery.OrderByDescending(v => v.Name)
                        : videosQuery.OrderBy(v => v.Name);
                    break;
                case "totaltime":
                    videosQuery = sortOrder.ToLower() == "desc"
                        ? videosQuery.OrderByDescending(v => v.TotalSeconds)
                        : videosQuery.OrderBy(v => v.TotalSeconds);
                    break;
                default: 
                    videosQuery = sortOrder.ToLower() == "desc"
                        ? videosQuery.OrderByDescending(v => v.Id)
                        : videosQuery.OrderBy(v => v.Id);
                    break;
            }

            int itemsToSkip = (pageNumber - 1) * pageSize;

            var videos = await videosQuery
                .Skip(itemsToSkip)
                .Take(pageSize)
                .ToListAsync();

            return videos;
        }


        // GET: api/Videos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Video>> GetVideo(int id)
        {
            if (_context.Videos == null)
            {
                return NotFound();
            }
            var video = await _context.Videos.FindAsync(id);

            if (video == null)
            {
                return NotFound();
            }

            return video;
        }

        // PUT: api/Videos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVideo(int id, Video video)
        {
            if (id != video.Id)
            {
                return BadRequest();
            }

            _context.Entry(video).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VideoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Videos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Video>> PostVideo(VideoDTO videoDTO)
        {
            if (_context.Videos == null)
            {
                return Problem("Entity set 'RwaMoviesContext.Videos'  is null.");
            }
            
            var video = _mapper.Map<Video>(videoDTO);    
            if (!videoDTO.VideoTags.IsNullOrEmpty())
            {
                video.VideoTags = videoDTO.VideoTags!.Select(id => new VideoTag { TagId = id }).ToList();
            }
            _context.Videos.Add(video);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVideo", new { id = video.Id }, video);
        }

        // DELETE: api/Videos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            if (_context.Videos == null)
            {
                return NotFound();
            }
            var video = await _context.Videos.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VideoExists(int id)
        {
            return (_context.Videos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

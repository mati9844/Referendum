using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class ReferendumsController : Controller
    {
        private readonly WebApplicationContext _context;

        public ReferendumsController(WebApplicationContext context)
        {
            _context = context;
        }

        // GET: Referendums
        public async Task<IActionResult> Index()
        {
            var webApplicationContext = _context.Referendums.Include(r => r.Question);
            return View(await webApplicationContext.ToListAsync());
        }

        // GET: Referendums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var referendum = await _context.Referendums
                .Include(r => r.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (referendum == null)
            {
                return NotFound();
            }

            return View(referendum);
        }

        // GET: Referendums/Create
        public IActionResult Create()
        {
            ViewData["QuestionID"] = new SelectList(_context.Questions, "Id", "Id");
            return View();
        }

        // POST: Referendums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,QuestionID,StartDate,EndDate")] Referendum referendum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(referendum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuestionID"] = new SelectList(_context.Questions, "Id", "Id", referendum.QuestionID);
            return View(referendum);
        }

        // GET: Referendums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var referendum = await _context.Referendums.FindAsync(id);
            if (referendum == null)
            {
                return NotFound();
            }
            ViewData["QuestionID"] = new SelectList(_context.Questions, "Id", "Id", referendum.QuestionID);
            return View(referendum);
        }

        // POST: Referendums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,QuestionID,StartDate,EndDate")] Referendum referendum)
        {
            if (id != referendum.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(referendum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReferendumExists(referendum.Id))
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
            ViewData["QuestionID"] = new SelectList(_context.Questions, "Id", "Id", referendum.QuestionID);
            return View(referendum);
        }

        // GET: Referendums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var referendum = await _context.Referendums
                .Include(r => r.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (referendum == null)
            {
                return NotFound();
            }

            return View(referendum);
        }

        // POST: Referendums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var referendum = await _context.Referendums.FindAsync(id);
            _context.Referendums.Remove(referendum);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReferendumExists(int id)
        {
            return _context.Referendums.Any(e => e.Id == id);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewTechParentPortalV3.Data;
using NewTechParentPortalV3.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NewTechParentPortalV3.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        
        private readonly UserManager<IdentityUser> _userManager;
        

        private readonly PortalContext _context;

        private int _currentParentID=0;

        public StudentsController(PortalContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            //var parents = from p in _context.Parents
            //              where p.Email.Contains(_userManager.GetUserName(User))
            //              select p.ID;
            //_currentParentID = _context.Parents.First(p => p.Email.Contains(_userManager.GetUserName(User))).ID;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        //public string Index()
        {

            string searchString = _userManager.GetUserName(User);
            //ViewData["CurrentFilter"] = searchString;

            var students = from s in _context.Students
                           select s;

            // the IsInRole command in the below line did not work on asp.core version 2.1; 
            if (!String.IsNullOrEmpty(searchString) && /*User.IsInRole("Parent"))*/searchString!= "admin@gmail.com")
            {
                students = students.Where(s => s.Parent.Email.Contains(searchString));
            }


            

            if (searchString != "admin@gmail.com")
            {
                if (_context.Parents.Where(p => p.Email.Contains(_userManager.GetUserName(User))).Count() == 0)
                {
                    Parent newParent = new Parent();
                    newParent.Email = _userManager.GetUserName(User);

                    _context.Add(newParent);
                    await _context.SaveChangesAsync();
                }


                _currentParentID = _context.Parents.First(p => p.Email.Contains(_userManager.GetUserName(User))).ID;
            }

            return View(await students.AsNoTracking().ToListAsync());
            //return (_currentParentID.ToString());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var student = await _context.Students
            //    .FirstOrDefaultAsync(m => m.StudentID == id);

            var student = await _context.Students
                .Include(s => s.Enrollments)
                   .ThenInclude(e => e.Course)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.StudentID == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("LastName,FirstMidName,EnrollmentDate,Age,DateOfBirth,Gender")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _currentParentID = _context.Parents.First(p => p.Email.Contains(_userManager.GetUserName(User))).ID;
                    student.ParentID = _currentParentID;
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "please email the problem to the maintenance service.");
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var studentToUpdate = await _context.Students.SingleOrDefaultAsync(s => s.StudentID == id);
            if (await TryUpdateModelAsync<Student>(
                studentToUpdate,
                "",
                s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate, s => s.Age, s => s.DateOfBirth, s => s.Gender))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "please email the problem to the maintenance service.");
                }
            }
            return View(studentToUpdate);
        }

        // GET: Students/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.StudentID == id);
            if (student == null)
            {
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "please email the problem to the maintenance service.";
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students
                //.FindAsync(id);
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.StudentID == id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

            private bool StudentExists(int id)
            {
                return _context.Students.Any(e => e.StudentID == id);
            }
        }
    }

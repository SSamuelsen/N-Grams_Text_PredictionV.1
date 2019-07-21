using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TextPrediction.Models;
using TextPrediction.Services;
using TextPrediction.Services.Interfaces;

namespace TextPrediction.Controllers
{
    public class SearchController : Controller
    {
        private readonly SearchContext _context;
        private readonly INgrams _Ngrams;

        public SearchController(SearchContext context, INgrams ngrams)
        {
            _context = context;
            _Ngrams = ngrams;
        }

        public static string SearchString;
        public static string Query;
        public static List<Search> Searches;


        // GET: Search
        public async Task<IActionResult> Index()
        {

            //SearchString = " "; //give this a default value to prevent againt a null being passed into the ngram function

            //retrieve the searches from the database
            Searches = _context.Searches.ToList();
            SearchString = _Ngrams.RetrieveSearches(Searches);
            

            return View(await _context.Searches.ToListAsync());


        }//end index


        [HttpPost]
        public ActionResult SaveSmartSearchProperties(string data)
        {

            dynamic results = JsonConvert.DeserializeObject<dynamic>(data);
            Query = results.data.Query;     //make the search query globally available
            SearchString = _Ngrams.RetrieveSearches(Searches);

            Search newSearch = new Search
            {
                Author = results.data.Author,
                Query = results.data.Query

            };



            if (ModelState.IsValid)
            {
                _context.Add(newSearch);
                _context.SaveChanges();
                
            }


            
            return Json(new { data = "works"});
        }//end save smart func


        [HttpPost]
        public ActionResult Ngrams(string data)
        {

            dynamic results = JsonConvert.DeserializeObject<dynamic>(data);
            string query = results.data.query;
           


            string searchString = SearchString;
            //searchString = "Stephen Samuelsen";
            //create the ngram model 
            var result = _Ngrams.CreateNgrams(searchString, 2);

            foreach(var item in result)
            {
                Debug.WriteLine(item.ToString());
            }

            if(query == null)
            {
                query = " ";
            }

            string prediction = " ";

            //perform logic to get the prediction 
            var smartPredict = from x in result
                               where x.ToUpper().Contains(query.ToUpper())
                               group x by x.ToUpper() into grp
                               orderby grp.Count() descending
                               select grp;

            //foreach(var item in smartPredict)
            //{
            //    Debug.WriteLine("Smart Predict" + item.FirstOrDefault() + item.Count());    //this is just for testing to see the n gram model
            //}



            if(smartPredict != null)
            {
                if(smartPredict.FirstOrDefault() != null)
                {
                    if(smartPredict.FirstOrDefault().Key.Split(' ')[1] != null)
                    {
                        string secondWord = smartPredict.FirstOrDefault().Key.Split(' ')[1];
                        //Debug.WriteLine("My smart prediction is: " + secondWord);               //write out the prediction for testing
                        prediction = secondWord;
                    }//end if
                }//end if
            }//end if 





            return Json(new { autocomplete = prediction });


        }//end Ngrams func









        // GET: Search/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var search = await _context.Searches
                .FirstOrDefaultAsync(m => m.SearchId == id);
            if (search == null)
            {
                return NotFound();
            }

            return View(search);
        }

        // GET: Search/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Search/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SearchId,Query,Author")] Search search)
        {
            if (ModelState.IsValid)
            {
                _context.Add(search);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(search);
        }

        // GET: Search/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var search = await _context.Searches.FindAsync(id);
            if (search == null)
            {
                return NotFound();
            }
            return View(search);
        }

        // POST: Search/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SearchId,Query,Author")] Search search)
        {
            if (id != search.SearchId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(search);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SearchExists(search.SearchId))
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
            return View(search);
        }

        // GET: Search/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var search = await _context.Searches
                .FirstOrDefaultAsync(m => m.SearchId == id);
            if (search == null)
            {
                return NotFound();
            }

            return View(search);
        }

        // POST: Search/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var search = await _context.Searches.FindAsync(id);
            _context.Searches.Remove(search);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SearchExists(int id)
        {
            return _context.Searches.Any(e => e.SearchId == id);
        }
    }
}

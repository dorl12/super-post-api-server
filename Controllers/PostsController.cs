using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Super_Post.Models;

namespace Super_Post.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private string _dataFilePath = "Data/posts.json";

        private List<Post> LoadPostsFromJsonFile()
        {
            string jsonData = System.IO.File.ReadAllText(_dataFilePath);
            List<Post>? posts = JsonConvert.DeserializeObject<List<Post>>(jsonData);
            return posts ?? new List<Post>();
        }

        private void SavePostsToJsonFile(List<Post> posts)
        {
            string jsonData = JsonConvert.SerializeObject(posts, Formatting.Indented);
            System.IO.File.WriteAllText(_dataFilePath, jsonData);
        }

        [HttpGet]
        public IActionResult Get()
        {
            // Retrieve and return posts from your JSON file
            List<Post> posts = LoadPostsFromJsonFile();
            return Ok(posts);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Post post)
        {
            if (post == null)
            {
                return BadRequest("Invalid post data.");
            }

            List<Post> posts = LoadPostsFromJsonFile();

            int maxId = posts.Count > 0 ? posts.Max(p => p.Id) : 0;

            post.Id = maxId + 1;
            DateTime currentDate = DateTime.Now;
            post.Date = currentDate.ToString("yyyy/MM/dd");
            posts.Add(post);
            SavePostsToJsonFile(posts);

            return CreatedAtAction(nameof(Get), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Post updatedPost)
        {
            if (updatedPost == null || id != updatedPost.Id)
            {
                return BadRequest("Invalid post data.");
            }

            List<Post> posts = LoadPostsFromJsonFile();
            Post? existingPost = posts.FirstOrDefault(p => p.Id == id);
            if (existingPost == null)
            {
                return NotFound();
            }

            // Update the fields
            existingPost.Title = updatedPost.Title;
            existingPost.Description = updatedPost.Description;
            existingPost.Category = updatedPost.Category;
            existingPost.Location = updatedPost.Location;
            existingPost.Picture = updatedPost.Picture;

            SavePostsToJsonFile(posts);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            List<Post> posts = LoadPostsFromJsonFile();
            Post? postToRemove = posts.FirstOrDefault(p => p.Id == id);
            if (postToRemove == null)
            {
                return NotFound();
            }

            posts.Remove(postToRemove);
            SavePostsToJsonFile(posts);

            return NoContent();
        }
    }
}

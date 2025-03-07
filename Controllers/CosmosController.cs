using CosmosApp.Models;
using CosmosApp.Models.Notes;
using CosmosApp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CosmosApp.Controllers
{
    [Route("Candidate")]
    public class CosmosController(CosmosCandidateService cosmosCandidateService,
        CosmosCandidateNotesService cosmosCandidateNotesService,
        CosmosCandidateNoteArchivesService cosmosCandidateNoteArchivesService) : Controller
    {
        private readonly CosmosCandidateService _cosmosCandidateService = cosmosCandidateService;
        private readonly CosmosCandidateNotesService _cosmosCandidateNotesService = cosmosCandidateNotesService;
        private readonly CosmosCandidateNoteArchivesService _cosmosCandidateNoteArchivesService = cosmosCandidateNoteArchivesService;


#region Candidate

        [HttpPost("Candidate")]
        public async Task<IActionResult> PostCandidate(Candidate candidate)
        {
            await _cosmosCandidateService.CreateCandidateAsync(candidate);
            return Ok();
        }

        [HttpPost("CandidateDefault")]
        public async Task<IActionResult> PostCandidateDefault()
        {
            await _cosmosCandidateService.CreateCandidateAsync(new Candidate());
            return Ok();
        }

        [HttpGet("Candidate/{id}")]
        public async Task<IActionResult> GetCandidate(string id)
        {
            Candidate candidate = await _cosmosCandidateService.GetCandidateAsync(id);
            string jsonresult = JsonConvert.SerializeObject(candidate);
            return Ok(jsonresult);
        }

/*
        [HttpDelete("Candidate")]
        public async Task<IActionResult> GetCandidate()
        {
            return Ok(_cosmosSetupService.DeleteAllContainersAsync());
        }
*/

#endregion

#region CandidateNotes

        [HttpPost("CandidateNote")]
        public async Task<IActionResult> PostCandidateNote(CandidateNote candidateNote)
        {
            await _cosmosCandidateNotesService.CreateCandidateNoteAsync(candidateNote);
            return Ok();
        }

        [HttpPost("CandidateNoteDefault")]
        public async Task<IActionResult> PostCandidateNoteDefault()
        {
            await _cosmosCandidateNotesService.CreateCandidateNoteAsync(new CandidateNote());
            return Ok();
        }

        [HttpGet("CandidateNote/{id}")]
        public async Task<IActionResult> GetCandidateNotes(string id)
        {
            IActionResult result;

            CandidateNote? candidateNote = await _cosmosCandidateNotesService.GetCandidateNoteAsync(id);
            result = candidateNote != null ? Ok(JsonConvert.SerializeObject(candidateNote)) : NotFound();

            return result;
        }

        [HttpPatch("CandidateNoteContent")]
        public async Task<IActionResult> PatchCandidateNotes(string id, string content)
        {
            IActionResult result;

            try
            {
                await _cosmosCandidateNotesService.UpdateCandidateNoteContentAsync(id, content);
                result = Ok();
            }
            catch (Exception ex)
            {
                if (ex.Message == "Note not found.")
                    result = NotFound();
                else
                    result = Problem(ex.Message);
            }
            
            return result;
        }


#endregion

#region CandidateNoteArchives
/*
        [HttpPost("CandidateNoteArchive")]
        public Task<IActionResult> PostCandidateNoteArchives(VersionedCandidateNote versionedCandidateNote)
        {
            return Ok(_cosmosCandidateNoteArchivesService.CreateCandidateNoteArchivesAsync(versionedCandidateNote));
        }
*/
        [HttpGet("CandidateNoteArchive/{id}")]
        public async Task<IActionResult> GetCandidateNoteArchives(string id)
        {
            return Ok(JsonConvert.SerializeObject(await _cosmosCandidateNoteArchivesService.GetCandidateNoteArchiveAsync(id)));
        }

#endregion

    }
}

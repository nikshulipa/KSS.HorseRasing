namespace KSS.HorseRacing.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using KSS.HorseRacing.Infrastucture.DataAccess;
    using KSS.HorseRacing.Infrastucture.DataModels;
    using KSS.HorseRacing.Models;

    public class JokeyService
    {
        private readonly GeneralService _generalService;

        public JokeyService(GeneralService generalService)
        {
            _generalService = generalService;
        }

        public IEnumerable<JockeyViewModel> GetListJokeys()
        {
            using (var unit = new UnitOfWork())
            {
                var jockeys = unit.Jockey.GetAllJockeys();
                var jockeysList = jockeys.Select(
                    jockey => new JockeyViewModel
                    {

                        Alias = jockey.Alias,
                        DateBirth = _generalService.GetDateTimeStringForDatepicker(jockey.DateBirth),
                        FirstName = jockey.FirstName,
                        JockeyId = jockey.Id,
                        LastName = jockey.LastName,
                        MiddleName = jockey.MiddleName
                    });
                return jockeysList;
            }
        }

        public JockeyViewModel GetJockeyDetails(int id)
        {
            using (var unit = new UnitOfWork())
            {
                var jockey = unit.Jockey.Get(id);
                var model = new JockeyViewModel
                {
                    JockeyId = jockey.Id,
                    Alias = jockey.Alias,
                    DateBirth = _generalService.GetDateTimeStringForDatepicker(jockey.DateBirth),
                    FirstName = jockey.FirstName,
                    LastName = jockey.LastName,
                    MiddleName = jockey.MiddleName
                };
                return model;
            }
        }

        public void AddNewJockey(JockeyViewModel model)
        {
            using (var unit = new UnitOfWork())
            {
                var jockey = new Jockey
                {
                    Alias = model.Alias,
                    DateBirth = DateTime.Parse(model.DateBirth),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MiddleName = model.MiddleName
                };
                unit.Jockey.Save(jockey);
            }
        }

        public void EditJockey(JockeyViewModel model)
        {
            using (var unit = new UnitOfWork())
            {
                var jockey = unit.Jockey.Get(model.JockeyId);
                jockey.Alias = model.Alias;
                jockey.DateBirth = DateTime.Parse(model.DateBirth);
                jockey.FirstName = model.FirstName;
                jockey.LastName = model.LastName;
                jockey.MiddleName = model.LastName;
                unit.Jockey.Save(jockey);
            }
        }

        public void DeleteJockey(int id)
        {
            using (var unit = new UnitOfWork())
            {
                var jockey = unit.Jockey.Get(id);
                unit.Jockey.Delete(jockey);
            }
        }

        public List<WinnerJockeyForYearViewModel> GetWinnerJokeysForYear(int year)
        {
            using (var unit = new UnitOfWork())
            {
                var participants = unit.Participant.GetWinnersForYear(year);
                var model = participants.Select(participant => new WinnerJockeyForYearViewModel
                    {
                        JockeyId = participant.Racer.JockeyId,
                        JockeyAlias = participant.Racer.Jockey.Alias,
                        QuantityWinnerRaces = unit.Participant.GetCountWinnerRaces(participant.Id),
                        JockeyFullName = participant.Racer.Jockey.FullName
                    }).OrderByDescending(x => x.JockeyFullName).ToList();
                return model;
            }
        }
    }
}
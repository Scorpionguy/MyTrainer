using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MyTrainer.Data;
using MyTrainer.Models;
using Microsoft.AspNetCore.Identity;


namespace MyTrainer.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private User _user;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public IActionResult Main()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Authorization", "Account");
            }

            _user = _context.Users
                .Include(u => u.weightHistory)
                .FirstOrDefault(u => u.Id == userId);

            if (_user.weightHistory.Count > 0)
            {
                var lastWeight = _user.weightHistory.OrderBy(wh => wh.updateTime).Last().weight;
                var model = new UserProgressViewModel
                {
                    goalWeight = _user.goalWeight,
                    startWeight = _user.startWeight,
                    CurrentWeight = lastWeight,
                    Weights = _user.weightHistory
                };
                return View(model);
            }
            else
            {
                var model = new UserProgressViewModel
                {
                    goalWeight = _user.goalWeight,
                    startWeight = _user.startWeight,
                    CurrentWeight = 0,
                    Weights = _user.weightHistory
                };
                return View(model);
            }
        }

        public async Task<IActionResult> AddWeight(UserProgressViewModel model)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var modelWeight = new Weight_history
            {
                userId = userId,
                weight = model.add.Weight,
                updateTime = DateTime.UtcNow
            };
            _context.WeightHistory.Add(modelWeight);
            
            await _context.SaveChangesAsync();
            TempData["AddedWeight"] = "Данные о взвешивании добавлены!";
            return RedirectToAction("Main", "Profile");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteWeight(int id)
        {
            try
            {
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				var model = _context.WeightHistory.FirstOrDefault(wh => wh.id == id);
				_context.WeightHistory.Remove(model);
				await _context.SaveChangesAsync();
				return RedirectToAction("Main", "Profile");
			}
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ошибка при удалении данных");
                return RedirectToAction("Main", "Profile");
			}
        }


        [HttpPost]
        public async Task<IActionResult> AddExercise(Exercise model)
        {
           

            var exerciseModel = new Exercise
            {
                name = model.name,
                howToUrl = model.howToUrl,
                standatdCal = model.standatdCal,
                description = model.description,
                targetMuscle = model.targetMuscle
            };

            _context.Exercise.Add(exerciseModel);
            await _context.SaveChangesAsync();
            TempData["AddedExercise"] = "Новое упражнение успешно добавлено!";
            return RedirectToAction("Main", "Profile");
        }

        public List<Activity_history> getLastActivities(string userId)
        {
            var lastActivitys = _context.ActivityHistory
                 .Where(ah => ah.userId == userId)
                 .OrderByDescending(ah => ah.startDateTime)
                 .Include(ah => ah.activityExercise)
                 .ThenInclude(ae => ae.Exercise)
                 .ToList();
            return lastActivitys;
        }


		[Authorize]
		public IActionResult Exercise(int index = 0)
        {
            ViewData["HideFooter"] = true;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Authorization", "Account");
            }

            var lastActivitys = getLastActivities(userId);

            if (index < 0) index = 0;
            if (index >= lastActivitys.Count) index = lastActivitys.Count - 1;

            ////тут все тренировки, чтобы можно было выбирать их по айдишнику через назад/вперед на странице, доделать
            //сделать так, чтобы при регистрации вновь указывались все нужные параметры для моментального отображения их в профиле, либо сделать костыль с *если нет веса то принудительно его добавить на странице
            if (lastActivitys == null) return RedirectToAction("Privacy", "Home");

            List<WorkoutsModel> workouts = new List<WorkoutsModel>();
            foreach (var workout in lastActivitys)
            {
                var exercises = new List<Activity_ExerciseViewModel>();
                exercises = workout.activityExercise.Select(e => new Activity_ExerciseViewModel
                {
                    ExerciseName = e.Exercise.name,
                    Sets = e.sets,
                    Reps = e.reps,
                    WeightUsed = e.weightUsed,
                    CalBurned = e.calBurned
                }).ToList();
                var model = new WorkoutsModel
                {
                    LatestWorkout = workout,
                    ExercisesOfLatestWorkout = exercises
                };
                workouts.Add(model);
            }

            var allmodel = new AllWorkoutsModel
            {
                Workouts = workouts,
                CurrentWorkoutIndex = index
            };

            //план: доставать список/словарь тренировок за сегодняшнее число, передавать на сайт, где через цикл заполняется таблица дропдаун тренировок за день, в которых будут дропдауны упражнений в тренировке
            return View("Exercise", allmodel);
        }
		[Authorize]
		public IActionResult StartWorkout()
        {
            var model = new StartWorkoutViewModel
            {
                AvailableExercises = _context.Exercise
                    .Where(e => e.confirmed)
                   .Select(e => new ExerciseInfo
                   {
                       Id = e.id,
                       Name = e.name,
                       Description = e.description,
                       HowToUrl = e.howToUrl,
                       calBurned = e.standatdCal
                   })
                   
                   .ToList()
            };
            return View(model);
        }

		[Authorize]
		[HttpPost]
        public IActionResult Save([FromBody] StartWorkoutInputModel input)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var history = new Activity_history
            {
                userId = userId,
                startDateTime = input.StartTime,
                endDateTime = DateTime.UtcNow,
                totalCal = input.burnedCals,
                coment = "Автоматически создано"
            };

            _context.ActivityHistory.Add(history);
            _context.SaveChanges();
            //на странице траблы с джс, он передается репы, но не сеты, а так же ничего не добавляет в активити_ексерс -- все круто теперь
            foreach (var ex in input.Exercises)
            {
                foreach (var set in ex.Sets)
                {
                    var activityExercise = new Activity_Exercise
                    {
                        activityId = history.id,
                        exerciseId = ex.ExerciseId,
                        reps = set.Reps,
                        weightUsed = set.Weight,
                        calBurned = set.calBurned
                    };
                    _context.ActivityExercise.Add(activityExercise);
                }
            }
            _context.SaveChanges();

            return Json(new { redirectUrl = Url.Action("Exercise", "Profile") });


        }

        public IActionResult PreviousWorkout(int currentIndex)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
            }
            return RedirectToAction("Exercise", new { index = currentIndex });
        }
        public IActionResult NextWorkout(int currentIndex)
        {
            currentIndex++;
            return RedirectToAction("Exercise", new { index = currentIndex });
        }
        public IActionResult DateWorkout(DateTime data)
        {
            var correctData = DateTime.SpecifyKind(data, DateTimeKind.Utc);

            var currentDay = _context.ActivityHistory
                .Where(ah => ah.endDateTime < correctData.AddDays(+1) && ah.endDateTime >= correctData.AddDays(-1)).OrderBy(ah => ah.startDateTime).LastOrDefault();
            //
            var lastActivities = getLastActivities(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var currentIndex = lastActivities.IndexOf(currentDay);
 

            return RedirectToAction("Exercise", new { index = currentIndex });
        }



        [Authorize]
		public IActionResult Personal()
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			_user = FindUser(userId);
			return View(_user);
		}
		[HttpPost]
		public IActionResult ChangeName(string nameChange, string lastNameChange)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			_user = FindUser(userId);

			if (nameChange != null)
			{
				_user.name = nameChange;
			}
			if (lastNameChange != null)
            {
				_user.lastname = lastNameChange;
			}
            
            _context.Users.Update(_user);
            _context.SaveChanges();
            TempData["DataSuccess"] = "Данные успешно обновлены!";
            return RedirectToAction("Personal");
        }
        [HttpPost]
        public IActionResult ChangeBirth(string changeBirth)
        {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _user = FindUser(userId);
            if (changeBirth != null)
            {
                _user.birthDay = DateTime.SpecifyKind(Convert.ToDateTime(changeBirth), DateTimeKind.Utc);
            }
			_context.Users.Update(_user);
			_context.SaveChanges();
			TempData["DataSuccess"] = "Данные успешно обновлены!";
			return RedirectToAction("Personal");
		}
        [HttpPost]
        public IActionResult ChangeGoals(double startChange, double goalChange)
        {
            if (startChange >= goalChange)
            {
                TempData["FailureWeight"] = "Вы не можете указать такие параметры веса!";
                return RedirectToAction("Personal");
            }
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			_user = FindUser(userId);
			if (startChange != 0)
			{
				_user.startWeight = Convert.ToDouble(startChange);
			}
			if (goalChange != 0)
			{
				_user.goalWeight = Convert.ToDouble(goalChange);
			}
			_context.Users.Update(_user);
			_context.SaveChanges();
            var modelWeight = new Weight_history
            {
                userId = userId,
                weight = startChange,
                updateTime = DateTime.UtcNow
            };
            _context.WeightHistory.Add(modelWeight);
            TempData["DataSuccess"] = "Данные успешно обновлены!";
			return RedirectToAction("Personal");
		}

		[HttpPost]
		public IActionResult ChangeEmail(string changeEmail)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			_user = FindUser(userId);
            if (changeEmail != null)
            {
                _user.Email = changeEmail;
            }
			_context.Users.Update(_user);
			_context.SaveChanges();
			TempData["DataSuccess"] = "Данные успешно обновлены!";
			return RedirectToAction("Personal");
		}

		[HttpPost]
        public IActionResult ChangePhone(string changePhone)
        {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			_user = FindUser(userId);
            if (changePhone != null)
            {
                _user.PhoneNumber = changePhone;
            }
			_context.Users.Update(_user);
			_context.SaveChanges();
			TempData["DataSuccess"] = "Данные успешно обновлены!";
			return RedirectToAction("Personal");
		}

		[Authorize]
		public IActionResult Reports()
		{
			return View();
		}

		[HttpGet]
		public IActionResult GetWeightChartData(DateTime startDate, DateTime endDate)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Unauthorized();

			// Загружаем записи веса за указанный период
			var weights = _context.WeightHistory
				.Where(w => w.userId == userId && w.updateTime >= DateTime.SpecifyKind(startDate, DateTimeKind.Utc) && w.updateTime < DateTime.SpecifyKind(endDate.AddDays(1), DateTimeKind.Utc))
				.OrderBy(w => w.updateTime)
				.Select(w => new
				{
					Date = w.updateTime.ToString("yyyy-MM-dd"),
					Weight = w.weight
				})
				.ToList();

			return Json(weights);
		}
		public IActionResult ChangePassword()
		{
			return View();
		}
        private User FindUser(string id)
        {
			_user = _context.Users
                .Include(u=> u.weightHistory)
				.FirstOrDefault(u => u.Id == id);
			if (_user == null)
			{
				return null;
			}
            return _user;
		}

        [Authorize]
        
        public IActionResult AdminPanel()
        {
            var model = new AdminDashboardViewModel
            {
                Users = _context.Users.Where(u => !u.EmailConfirmed).ToList(),
                Exercises = _context.Exercise.Where(e => e.confirmed == true).ToList(),
                NotConfirmed = _context.Exercise.Where(e => e.confirmed == false).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmUser(string id)
        {
            var user = _context.Users.Where(u => u.Id == id).FirstOrDefault();
                if (user != null && !user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    _context.Users.Update(user);
                    _context.SaveChanges();
                }
            
            
            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmExercise(int id)
        {
            var exercise = _context.Exercise.Where(u => u.id == id).FirstOrDefault();
            if (exercise != null && !exercise.confirmed)
            {
                exercise.confirmed = true;
                _context.Exercise.Update(exercise);
                _context.SaveChanges();
            }


            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        
        public async Task<IActionResult> DeleteExercise(int id)
        {
            var exercise = await _context.Exercise.FindAsync(id);
            if (exercise != null)
            {
                _context.Exercise.Remove(exercise);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminPanel");
        }
        [HttpPost]

        public async Task<IActionResult> DeleteNotConfirm(int id)
        {
            var exercise = await _context.Exercise.FindAsync(id);
            if (exercise != null)
            {
                _context.Exercise.Remove(exercise);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        public async Task<IActionResult> EditExercise(Exercise model)
        {
            model.confirmed = true;
            _context.Exercise.Update(model);
            _context.SaveChanges();
            return RedirectToAction("AdminPanel");
        }
    }
}

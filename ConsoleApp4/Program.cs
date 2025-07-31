using PokemonReviewApp.Dto;
using System.Text.Json;

namespace ConsoleApp4
{
    class Program
    {
        static async Task Main()
        {
            while (true)
            {
                Console.Clear();

                
                Console.WriteLine("Bir kategori seçin:");
                Console.WriteLine("1. Category");
                Console.WriteLine("2. Pokemon");
                Console.WriteLine("3. Owner");
                Console.WriteLine("4. Review");
                Console.WriteLine("5. Reviewer");
                Console.WriteLine("6. Country");
                string categoryChoice = GetValidInput("Kategori seçin (1-6): ", 1, 6);

                
                Console.WriteLine("\nBir işlem seçin:");
                Console.WriteLine("1. GET");
                Console.WriteLine("2. POST");
                Console.WriteLine("3. PUT");
                Console.WriteLine("4. DELETE");
                string methodChoice = GetValidInput("İşlem seçin (1-4): ", 1, 4);

                
                switch (categoryChoice)
                {
                    case "1":
                        await HandleCategoryMethod(methodChoice);  
                        break;
                    case "2":
                        await HandlePokemonMethod(methodChoice);  
                        break;
                    case "3":
                        await HandleOwnerMethod(methodChoice);    
                        break;
                    case "4":
                        await HandleReviewMethod(methodChoice);   
                        break;

                    case "5":
                        await HandleReviewerMethod(methodChoice); 
                        break;
                    case "6":
                        await HandleCountryMethod(methodChoice);  
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçim.");
                        break;
                }

                Console.WriteLine("\nDevam etmek için bir tuşa bas...");
                Console.ReadKey();
            }
        }

        
        static string GetValidInput(string prompt, int minValue, int maxValue)
        {
            string input = string.Empty;
            bool validInput = false;

            while (!validInput)
            {
                Console.Write(prompt);
                input = Console.ReadLine();

                if (int.TryParse(input, out int choice) && choice >= minValue && choice <= maxValue)
                {
                    validInput = true;
                }
                else
                {
                    Console.WriteLine($"Geçersiz seçim! Lütfen {minValue}-{maxValue} arasında bir sayı girin.");
                }
            }

            return input;
        }
        static async Task HandlePokemonMethod(string method)
        {
            switch (method)
            {
                case "1":  // GET
                    await GetPokemonAsync();
                    break;
                case "2":  // POST
                    await CreatePokemonAsync();
                    break;
                case "3":  // PUT
                    await UpdatePokemonAsync();
                    break;
                case "4":  // DELETE
                    await DeletePokemonAsync();
                    break;
                default:
                    Console.WriteLine("Geçersiz işlem.");
                    break;
            }
        }

        static async Task GetPokemonAsync()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7091/api/Pokemon");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"GET isteği başarısız oldu: {response.StatusCode}");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var pokemonList = JsonSerializer.Deserialize<List<PokemonDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine("\nPokemon Listesi:");
            foreach (var pokemon in pokemonList)
            {
                Console.WriteLine($"ID: {pokemon.Id}, Name: {pokemon.Name}, Birth Date: {pokemon.BirthDate:yyyy-MM-dd}");
            }
        }

        static async Task CreatePokemonAsync()
        {
            try
            {
                // Pokemon'un adını alıyoruz
                Console.Write("Pokemon Name: ");
                string name = Console.ReadLine();

                // Pokemon'un doğum tarihini alıyoruz ve kontrol ediyoruz
                Console.Write("Pokemon Birth Date (yyyy-MM-dd): ");
                DateTime birthDate;

                if (!DateTime.TryParse(Console.ReadLine(), out birthDate))
                {
                    Console.WriteLine("Geçersiz tarih formatı. Lütfen doğru formatta tarih giriniz.");
                    return;
                }

                // OwnerId'yi alıyoruz
                Console.Write("OwnerId (PokemonOwnerId): ");
                int ownerId;

                if (!int.TryParse(Console.ReadLine(), out ownerId))
                {
                    Console.WriteLine("Geçersiz OwnerId. Lütfen geçerli bir sayı giriniz.");
                    return;
                }

                // CategoryId'yi alıyoruz
                Console.Write("CategoryId (PokemonCategoryId): ");
                int catId;

                if (!int.TryParse(Console.ReadLine(), out catId))
                {
                    Console.WriteLine("Geçersiz CategoryId. Lütfen geçerli bir sayı giriniz.");
                    return;
                }

                // Yeni Pokemon nesnesi oluşturuluyor
                var newPokemon = new PokemonDto
                {
                    Name = name,
                    BirthDate = birthDate
                    // PokemonOwnerId ve CategoryId'yi burada almayacağız, bunlar query string olarak ekleniyor
                };

                // HttpClient ile API'ye POST isteği gönderiyoruz
                using (var client = new HttpClient())
                {
                    // PokemonDto nesnesini JSON'a dönüştürüyoruz
                    var jsonContent = JsonSerializer.Serialize(newPokemon);
                    var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                    // URL'yi query parametreleri ile oluşturma (ownerId ve catId'yi query parametreleri olarak gönderiyoruz)
                    var url = $"https://localhost:7091/api/Pokemon?ownerId={ownerId}&catId={catId}";

                    // POST isteğini gönderiyoruz
                    var response = await client.PostAsync(url, content);

                    // HTTP yanıtını kontrol ediyoruz
                    if (response.IsSuccessStatusCode)
                        Console.WriteLine("Pokemon başarıyla eklendi!");
                    else
                        Console.WriteLine($"POST isteği başarısız oldu: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Hata mesajı
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            }
        }




        static async Task UpdatePokemonAsync()
        {
            Console.Write("Güncellem    ek istediğiniz Pokemon ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Yeni Pokemon Name: ");
            string name = Console.ReadLine();

            Console.Write("Yeni Pokemon Birth Date (yyyy-MM-dd): ");
            DateTime birthDate = DateTime.Parse(Console.ReadLine());

            var updatedPokemon = new PokemonDto { Id = id, Name = name, BirthDate = birthDate };

            var client = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(updatedPokemon);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"https://localhost:7091/api/Pokemon/{id}", content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Pokemon başarıyla güncellendi!");
            else
                Console.WriteLine($"PUT isteği başarısız oldu: {response.StatusCode}");
        }

        static async Task DeletePokemonAsync()
        {
            Console.Write("Silmek istediğiniz Pokemon ID: ");
            int id = int.Parse(Console.ReadLine());

            var client = new HttpClient();
            var response = await client.DeleteAsync($"https://localhost:7091/api/Pokemon/{id}");

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Pokemon başarıyla silindi.");
            else
                Console.WriteLine($"DELETE isteği başarısız oldu: {response.StatusCode}");
        }
        static async Task HandleCategoryMethod(string method)
        {
            switch (method)
            {
                case "1":  // GET
                    await GetCategoriesAsync();
                    break;
                case "2":  // POST
                    await CreateCategoryAsync();
                    break;
                case "3":  // PUT
                    await UpdateCategoryAsync();
                    break;
                case "4":  // DELETE
                    await DeleteCategoryAsync();
                    break;
                default:
                    Console.WriteLine("Geçersiz işlem.");
                    break;
            }
        }

        static async Task GetCategoriesAsync()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7091/api/Category");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"GET isteği başarısız oldu: {response.StatusCode}");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var categories = JsonSerializer.Deserialize<List<CategoryDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine("\nKategori Listesi:");
            foreach (var category in categories)
            {
                Console.WriteLine($"ID: {category.Id}, Name: {category.Name}");
            }
        }

        static async Task CreateCategoryAsync()
        {
            Console.Write("Kategori Adı: ");
            string name = Console.ReadLine();

            var newCategory = new CategoryDto { Name = name };

            var client = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(newCategory);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7091/api/Category", content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Kategori başarıyla eklendi!");
            else
                Console.WriteLine($"POST isteği başarısız oldu: {response.StatusCode}");
        }

        static async Task UpdateCategoryAsync()
        {
            Console.Write("Güncellemek istediğiniz Kategori ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Yeni Kategori Adı: ");
            string name = Console.ReadLine();

            var updatedCategory = new CategoryDto { Id = id, Name = name };

            var client = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(updatedCategory);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"https://localhost:7091/api/Category/{id}", content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Kategori başarıyla güncellendi!");
            else
                Console.WriteLine($"PUT isteği başarısız oldu: {response.StatusCode}");
        }

        static async Task DeleteCategoryAsync()
        {
            Console.Write("Silmek istediğiniz Kategori ID: ");
            int id = int.Parse(Console.ReadLine());

            var client = new HttpClient();
            var response = await client.DeleteAsync($"https://localhost:7091/api/Category/{id}");

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Kategori başarıyla silindi.");
            else
                Console.WriteLine($"DELETE isteği başarısız oldu: {response.StatusCode}");
        }


        // --- OWNER ---
        static async Task HandleOwnerMethod(string method)
        {
            switch (method)
            {
                case "1":  // GET
                    await GetOwnersAsync();
                    break;
                case "2":  // POST
                    await CreateOwnerAsync();
                    break;
                case "3":  // PUT
                    await UpdateOwnerAsync();
                    break;
                case "4":  // DELETE
                    await DeleteOwnerAsync();
                    break;
                default:
                    Console.WriteLine("Geçersiz işlem.");
                    break;
            }
        }

        static async Task GetOwnersAsync()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7091/api/Owner");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"GET isteği başarısız oldu: {response.StatusCode}");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var owners = JsonSerializer.Deserialize<List<OwnerDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine("\nOwner Listesi:");
            foreach (var owner in owners)
                Console.WriteLine($"ID: {owner.Id}, First Name: {owner.FirstName}, Last Name: {owner.LastName}, Gym: {owner.Gym}");
        }

        static async Task CreateOwnerAsync()
        {
            Console.Write("Owner First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Owner Last Name: ");
            string lastName = Console.ReadLine();

            Console.Write("Gym: ");
            string gym = Console.ReadLine();

            var client = new HttpClient();
            var newOwner = new OwnerDto { FirstName = firstName, LastName = lastName, Gym = gym };
            var jsonContent = JsonSerializer.Serialize(newOwner);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7091/api/Owner", content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Owner başarıyla eklendi!");
            else
                Console.WriteLine($"POST isteği başarısız oldu: {response.StatusCode}");
        }

        static async Task UpdateOwnerAsync()
        {
            Console.Write("Güncellemek istediğiniz Owner ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Yeni Owner First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Yeni Owner Last Name: ");
            string lastName = Console.ReadLine();

            Console.Write("Yeni Gym: ");
            string gym = Console.ReadLine();

            var client = new HttpClient();
            var updatedOwner = new OwnerDto { Id = id, FirstName = firstName, LastName = lastName, Gym = gym };
            var jsonContent = JsonSerializer.Serialize(updatedOwner);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"https://localhost:7091/api/Owner/{id}", content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Owner başarıyla güncellendi!");
            else
                Console.WriteLine($"PUT isteği başarısız oldu: {response.StatusCode}");
        }

        static async Task DeleteOwnerAsync()
        {
            Console.Write("Silmek istediğiniz Owner ID: ");
            int id = int.Parse(Console.ReadLine());

            var client = new HttpClient();
            var response = await client.DeleteAsync($"https://localhost:7091/api/Owner/{id}");

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Owner başarıyla silindi.");
            else
                Console.WriteLine($"DELETE isteği başarısız oldu: {response.StatusCode}");
        }

        // --- REVIEW ---
        static async Task HandleReviewMethod(string method)
        {
            switch (method)
            {
                case "1":  // GET
                    await GetReviewsAsync();
                    break;
                case "2":  // POST
                    await CreateReviewAsync();
                    break;
                case "3":  // PUT
                    await UpdateReviewAsync();
                    break;
                case "4":  // DELETE
                    await DeleteReviewAsync();
                    break;
                default:
                    Console.WriteLine("Geçersiz işlem.");
                    break;
            }
        }

        static async Task GetReviewsAsync()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7091/api/Review");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"GET isteği başarısız oldu: {response.StatusCode}");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var reviews = JsonSerializer.Deserialize<List<ReviewDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine("\nReview Listesi:");
            foreach (var r in reviews)
                Console.WriteLine($"ID: {r.Id}, Title: {r.Title}, Text: {r.Text}, Rating: {r.Rating}");
        }

        static async Task CreateReviewAsync()
        {
            try
            {
                // Kullanıcıdan giriş alma
                Console.Write("Başlık: ");
                string title = Console.ReadLine();

                Console.Write("Metin: ");
                string text = Console.ReadLine();

                Console.Write("Değerlendirme (1-5): ");
                if (!int.TryParse(Console.ReadLine(), out int rating) || rating < 1 || rating > 5)
                {
                    Console.WriteLine("Geçersiz değerlendirme! 1-5 arasında bir sayı girin.");
                    return;
                }

                Console.Write("Reviewer ID: ");
                if (!int.TryParse(Console.ReadLine(), out int reviewerId))
                {
                    Console.WriteLine("Geçersiz Reviewer ID! Bir sayı girin.");
                    return;
                }

                Console.Write("Pokemon ID: ");
                if (!int.TryParse(Console.ReadLine(), out int pokeId))
                {
                    Console.WriteLine("Geçersiz Pokemon ID! Bir sayı girin.");
                    return;
                }

                // ReviewDto'yu oluşturma
                var newReview = new ReviewDto { Title = title, Text = text, Rating = rating };

                // JSON verisini hazırlama
                var jsonContent = JsonSerializer.Serialize(newReview);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                // URL'yi query parametreleri ile oluşturma
                var url = $"https://localhost:7091/api/Review?reviewerId={reviewerId}&pokeId={pokeId}";

                // POST isteğini gönderme
                var client = new HttpClient();
                var response = await client.PostAsync(url, content);

                // Yanıtı işleme
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Review başarıyla eklendi!");
                }
                else
                {
                    // Hata mesajını al ve yazdır
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"POST isteği başarısız oldu: {response.StatusCode}");
                    Console.WriteLine($"Hata mesajı: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            }
        }

        static async Task UpdateReviewAsync()
        {
            Console.Write("Güncellemek istediğiniz Review ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Yeni başlık: ");
            string title = Console.ReadLine();

            Console.Write("Yeni metin: ");
            string text = Console.ReadLine();

            Console.Write("Yeni değerlendirme: ");
            int rating = int.Parse(Console.ReadLine());

            var updatedReview = new ReviewDto { Id = id, Title = title, Text = text, Rating = rating };

            var client = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(updatedReview);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"https://localhost:7091/api/Review/{id}", content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Review başarıyla güncellendi!");
            else
                Console.WriteLine($"PUT isteği başarısız oldu: {response.StatusCode}");
        }


        static async Task DeleteReviewAsync()
        {
            Console.Write("Silmek istediğiniz Review ID: ");
            int id = int.Parse(Console.ReadLine());

            var client = new HttpClient();
            var response = await client.DeleteAsync($"https://localhost:7091/api/Review/{id}");

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Review başarıyla silindi.");
            else
                Console.WriteLine($"DELETE isteği başarısız oldu: {response.StatusCode}");
        }


        static async Task HandleReviewerMethod(string method)
        {

            switch (method)
            {
                case "1":  // GET
                    await GetReviewersAsync();
                    break;
                case "2":  // POST
                    await CreateReviewerAsync();
                    break;
                case "3":  // PUT
                    await UpdateReviewerAsync();
                    break;
                case "4":  // DELETE
                    await DeleteReviewerAsync();
                    break;
                default:
                    Console.WriteLine("Geçersiz işlem.");
                    break;
            }
        }

        static async Task GetReviewersAsync()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7091/api/Reviewer");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"GET isteği başarısız oldu: {response.StatusCode}");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var reviewers = JsonSerializer.Deserialize<List<ReviewerDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine("\nReviewer Listesi:");
            foreach (var r in reviewers)
                Console.WriteLine($"ID: {r.Id}, First Name: {r.FirstName}, Last Name: {r.LastName}");
        }
        static async Task CreateReviewerAsync()
        {
            Console.Write("First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last Name: ");
            string lastName = Console.ReadLine();

            // ReviewerDto'ya uygun şekilde yeni bir nesne oluşturuyoruz
            var newReviewer = new ReviewerDto { FirstName = firstName, LastName = lastName };

            var client = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(newReviewer);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7091/api/Reviewer", content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Reviewer başarıyla eklendi!");
            else
                Console.WriteLine($"POST isteği başarısız oldu: {response.StatusCode}");
        }


        static async Task UpdateReviewerAsync()
        {
            Console.Write("Güncellemek istediğiniz Reviewer ID: ");
            int id = int.Parse(Console.ReadLine());

            // ReviewerDto'da FirstName ve LastName olduğu için bunları alacağız
            Console.Write("Yeni First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Yeni Last Name: ");
            string lastName = Console.ReadLine();

            // ReviewerDto'ya uygun şekilde nesne oluşturuyoruz
            var updatedReviewer = new ReviewerDto { Id = id, FirstName = firstName, LastName = lastName };

            var client = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(updatedReviewer);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            // PUT isteği ile güncelleme işlemi
            var response = await client.PutAsync($"https://localhost:7091/api/Reviewer/{id}", content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Reviewer başarıyla güncellendi!");
            else
                Console.WriteLine($"PUT isteği başarısız oldu: {response.StatusCode}");
        }


        static async Task DeleteReviewerAsync()
        {
            Console.Write("Silmek istediğiniz Reviewer ID: ");
            int id = int.Parse(Console.ReadLine());

            var client = new HttpClient();
            var response = await client.DeleteAsync($"https://localhost:7091/api/Reviewer/{id}");

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Reviewer başarıyla silindi.");
            else
                Console.WriteLine($"DELETE isteği başarısız oldu: {response.StatusCode}");
        }

        // --- COUNTRY ---
        static async Task HandleCountryMethod(string method)
        {
            switch (method)
            {
                case "1":  // GET
                    await GetCountriesAsync();
                    break;
                case "2":  // POST
                    await CreateCountryAsync();
                    break;
                case "3":  // PUT
                    await UpdateCountryAsync();
                    break;
                case "4":  // DELETE
                    await DeleteCountryAsync();
                    break;
                default:
                    Console.WriteLine("Geçersiz işlem.");
                    break;
            }
        }

        static async Task GetCountriesAsync()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7091/api/Country");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"GET isteği başarısız oldu: {response.StatusCode}");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var countries = JsonSerializer.Deserialize<List<CountryDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine("\nCountry Listesi:");
            foreach (var country in countries)
            {
                Console.WriteLine($"ID: {country.Id}, Name: {country.Name}");
            }
        }

        static async Task CreateCountryAsync()
        {
            Console.Write("Ülke Adı: ");
            string name = Console.ReadLine();

            var newCountry = new CountryDto { Name = name };

            var client = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(newCountry);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7091/api/Country", content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Ülke başarıyla eklendi!");
            else
                Console.WriteLine($"POST isteği başarısız oldu: {response.StatusCode}");
        }

        static async Task UpdateCountryAsync()
        {
            Console.Write("Güncellemek istediğiniz Ülke ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Yeni Ülke Adı: ");
            string name = Console.ReadLine();

            var updatedCountry = new CountryDto { Id = id, Name = name };

            var client = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(updatedCountry);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"https://localhost:7091/api/Country/{id}", content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Ülke başarıyla güncellendi!");
            else
                Console.WriteLine($"PUT isteği başarısız oldu: {response.StatusCode}");
        }

        static async Task DeleteCountryAsync()
        {
            Console.Write("Silmek istediğiniz Ülke ID: ");
            int id = int.Parse(Console.ReadLine());

            var client = new HttpClient();
            var response = await client.DeleteAsync($"https://localhost:7091/api/Country/{id}");

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Ülke başarıyla silindi.");
            else
                Console.WriteLine($"DELETE isteği başarısız oldu: {response.StatusCode}");
        }

    }
}
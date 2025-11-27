using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using MISA.CRM.Core.DTOs.Customer;

namespace MISA.CRM.Infrastructure.Seeders
{
    /// <summary>
    /// Seeder tạo dữ liệu khách hàng giả lập và xuất ra file CSV.
    /// </summary>
    /// CreatedBy: NTT (19/11/2025)
    public static class CustomerSeeder
    {
        /// <summary>
        /// Sinh file CSV khách hàng giả lập theo yêu cầu:
        /// - count >=1000
        /// -3 nhóm customerType: NBH01, LKHA, VIP
        /// -30% có lastPurchaseDate trong30 ngày gần nhất
        /// -5% có phone duplicate:1 master record + N duplicates reuse its phone
        /// -95% không có email/phone
        /// </summary>
        /// <param name="count">Số lượng bản ghi</param>
        /// <param name="outputPath">Đường dẫn file CSV xuất ra</param>
        public static void GenerateCsv(int count, string outputPath)
        {
            if (count <1000) count =1000;

            var rnd = new Random();
            var types = new[] { "NBH01", "LKHA", "VIP" };

            // Rates
            var recentRate =0.30; //30% recent purchase in last30 days

            // Number of duplicate error records we want (5%)
            var duplicateCount = (int)Math.Round(count *0.05);

            var faker = new Faker("vi");
            var phoneFaker = new Faker();

            // choose one master phone and indices for duplicates
            string masterPhone = string.Empty;
            int masterIndex = -1;
            var duplicateIndices = new HashSet<int>();
            if (duplicateCount >0)
            {
                masterPhone = GeneratePhone(phoneFaker);
                // pick master index randomly
                masterIndex = phoneFaker.Random.Int(0, count -1);
                // pick duplicateCount distinct indices != masterIndex
                while (duplicateIndices.Count < duplicateCount)
                {
                    var idx = phoneFaker.Random.Int(0, count -1);
                    if (idx == masterIndex) continue;
                    duplicateIndices.Add(idx);
                }
            }

            // Vietnamese streets/cities to produce realistic addresses
            var streets = new[] {
                "Phố Huế", "Trần Hưng Đạo", "Lý Thường Kiệt", "Nguyễn Trãi", "Hai Bà Trưng",
                "Đinh Tiên Hoàng", "Lê Hồng Phong", "Ngô Quyền", "Nguyễn Du", "Trần Phú"
            };
            var cities = new[] { "Hà Nội", "Hồ Chí Minh", "Đà Nẵng", "Hải Phòng", "Cần Thơ", "Nha Trang" };

            using var writer = new StreamWriter(outputPath, false, new UTF8Encoding(true));
            // header matches parser (use "Địa chỉ" to match ColumnMapping)
            writer.WriteLine("Họ và tên,Số điện thoại,Email,Địa chỉ,Loại khách hàng,Mã số thuế,Ngày mua gần nhất,Hàng hóa đã mua,Hàng hóa mua gần nhất");

            for (int i =0; i < count; i++)
            {
                // Generate full name and then move family name to front if needed
                var fullName = faker.Name.FullName();
                var nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (nameParts.Length >1)
                {
                    // Put last token (assumed family name) first, then the rest
                    fullName = nameParts[^1] + " " + string.Join(' ', nameParts.Take(nameParts.Length -1));
                }

                var type = types[i % types.Length]; // distribute types round-robin

                string phone = string.Empty;
                // Per request: keep emails empty to avoid duplicates
                string email = string.Empty;

                if (i == masterIndex || duplicateIndices.Contains(i))
                {
                    phone = masterPhone;
                }

                string taxCodeRaw = faker.Random.Replace("##########");

                string lastPurchase = string.Empty;
                if (phoneFaker.Random.Double() < recentRate)
                {
                    var daysAgo = phoneFaker.Random.Int(0,29);
                    lastPurchase = DateTime.Now.AddDays(-daysAgo).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    if (phoneFaker.Random.Double() <0.6)
                    {
                        var daysAgo = phoneFaker.Random.Int(31,365 *2);
                        lastPurchase = DateTime.Now.AddDays(-daysAgo).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        lastPurchase = string.Empty;
                    }
                }

                var address = $"{rnd.Next(1,999)} {streets[rnd.Next(streets.Length)]}, {cities[rnd.Next(cities.Length)]}";
                var purchased = phoneFaker.Random.ArrayElement(new[] { "LAPTOP", "XEMAY", "DIENTHOAI", "BANPHIM", "TUISACH" });
                var latest = purchased.ToLower();

                // Format phone and taxCode using leading single quote to preserve leading zeros in Excel
                string phoneForCsv = string.IsNullOrWhiteSpace(phone) ? string.Empty : "'" + phone;
                string taxCodeForCsv = string.IsNullOrWhiteSpace(taxCodeRaw) ? string.Empty : "'" + taxCodeRaw;

                string Escape(string s) => s == null ? string.Empty : (s.Contains(',') || s.Contains('"') ? "\"" + s.Replace("\"", "\"\"") + "\"" : s);

                var row = new[] {
                    Escape(fullName),
                    Escape(phoneForCsv),
                    Escape(email),
                    Escape(address),
                    Escape(type),
                    Escape(taxCodeForCsv),
                    Escape(lastPurchase),
                    Escape(purchased),
                    Escape(latest)
                };

                writer.WriteLine(string.Join(',', row));
            }

            writer.Flush();
        }

        private static string GeneratePhone(Faker f)
        {
            var prefixes = new[] { "09", "03", "07", "08", "05" };
            var prefix = f.Random.ArrayElement(prefixes);
            var restLen = f.Random.Int(8,9); // total length10 or11
            var sb = new StringBuilder(prefix);
            for (int i =0; i < restLen; i++) sb.Append(f.Random.Int(0,9));
            return sb.ToString();
        }

        
    }
}

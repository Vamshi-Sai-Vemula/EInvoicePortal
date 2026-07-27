using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenxOCC.Data.Entities
{
    public class Configuration
    {

        public int Id { get; set; }

        // --- UAE TEST B2B ---
        public string uae_test_b2b_tokenUrl { get; set; }
        public string uae_test_b2b_uuidsUrl { get; set; }
        public string uae_test_b2b_password { get; set; }
        public string uae_test_b2b_profileId { get; set; }
        public string uae_test_b2b_postingUrl { get; set; }
        public string uae_test_b2b_username { get; set; }
        public string uae_test_b2b_creditMemoUrl { get; set; }
        public string uae_test_b2b_apiKey { get; set; }

        // --- UAE TEST B2C ---
        public string uae_test_b2c_tokenUrl { get; set; }
        public string uae_test_b2c_uuidsUrl { get; set; }
        public string uae_test_b2c_password { get; set; }
        public string uae_test_b2c_profileId { get; set; }
        public string uae_test_b2c_postingUrl { get; set; }
        public string uae_test_b2c_username { get; set; }
        public string uae_test_b2c_creditMemoUrl { get; set; }
        public string uae_test_b2c_apiKey { get; set; }

        // --- UAE LIVE B2B ---
        public string uae_live_b2b_tokenUrl { get; set; }
        public string uae_live_b2b_uuidsUrl { get; set; }
        public string uae_live_b2b_password { get; set; }
        public string uae_live_b2b_profileId { get; set; }
        public string uae_live_b2b_postingUrl { get; set; }
        public string uae_live_b2b_username { get; set; }
        public string uae_live_b2b_creditMemoUrl { get; set; }
        public string uae_live_b2b_apiKey { get; set; }

        // --- UAE LIVE B2C ---
        public string uae_live_b2c_tokenUrl { get; set; }
        public string uae_live_b2c_uuidsUrl { get; set; }
        public string uae_live_b2c_password { get; set; }
        public string uae_live_b2c_profileId { get; set; }
        public string uae_live_b2c_postingUrl { get; set; }
        public string uae_live_b2c_username { get; set; }
        public string uae_live_b2c_creditMemoUrl { get; set; }
        public string uae_live_b2c_apiKey { get; set; }

        // --- SAUDI TEST B2B ---
        public string sa_test_b2b_tokenUrl { get; set; }
        public string sa_test_b2b_uuidsUrl { get; set; }
        public string sa_test_b2b_password { get; set; }
        public string sa_test_b2b_profileId { get; set; }
        public string sa_test_b2b_postingUrl { get; set; }
        public string sa_test_b2b_username { get; set; }
        public string sa_test_b2b_creditMemoUrl { get; set; }
        public string sa_test_b2b_apiKey { get; set; }

        // --- SAUDI TEST B2C ---
        public string sa_test_b2c_tokenUrl { get; set; }
        public string sa_test_b2c_uuidsUrl { get; set; }
        public string sa_test_b2c_password { get; set; }
        public string sa_test_b2c_profileId { get; set; }
        public string sa_test_b2c_postingUrl { get; set; }
        public string sa_test_b2c_username { get; set; }
        public string sa_test_b2c_creditMemoUrl { get; set; }
        public string sa_test_b2c_apiKey { get; set; }

        // --- SAUDI LIVE B2B ---
        public string sa_live_b2b_tokenUrl { get; set; }
        public string sa_live_b2b_uuidsUrl { get; set; }
        public string sa_live_b2b_password { get; set; }
        public string sa_live_b2b_profileId { get; set; }
        public string sa_live_b2b_postingUrl { get; set; }
        public string sa_live_b2b_username { get; set; }
        public string sa_live_b2b_creditMemoUrl { get; set; }
        public string sa_live_b2b_apiKey { get; set; }

        // --- SAUDI LIVE B2C ---
        public string sa_live_b2c_tokenUrl { get; set; }
        public string sa_live_b2c_uuidsUrl { get; set; }
        public string sa_live_b2c_password { get; set; }
        public string sa_live_b2c_profileId { get; set; }
        public string sa_live_b2c_postingUrl { get; set; }
        public string sa_live_b2c_username { get; set; }
        public string sa_live_b2c_creditMemoUrl { get; set; }
        public string sa_live_b2c_apiKey { get; set; }

        public int? CompanyId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string UpdatedBy { get; set; }
    }
}

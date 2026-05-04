using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Core.ConfigOption
{
    public class JwtTokenSettings
    {
        public string SecretKey { get; set; } = null!; // key để mã hóa token, nên được giữ bí mật và có độ dài đủ lớn để đảm bảo an toàn
        public string? Issuer { get; set; } // tên của tổ chức hoặc ứng dụng phát hành token, thường được sử dụng để xác định nguồn gốc của token
        public int ExpireHours { get; set; } // thời gian hết hạn của token, tính bằng giờ
       
    }
}

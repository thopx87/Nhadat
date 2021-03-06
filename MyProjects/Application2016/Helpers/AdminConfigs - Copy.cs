using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application2016.Helpers
{
    public static class AdminConfigs
    {
        // Tên website, công ty
        public const string WEBSITE_NAME = "Nhà đất 10 tỷ (nhadat10ty.com)";

        // Mặc định số bản ghi hiển thị trong mỗi trang Admin
        public const int PAGE_SIZE = 10;

        // Mặc định số trang được show ra.
        public const int PAGE_SHOW = 5;

        // Mặc định số trang lùi lại được in ra so với trang hiện tại.
        // Eg: Trang hiện tại là trang 6 trong 100 trang.
        //      PAGE_SHOW = 5 --> 5 trang được in ra là: 4, 5, 6, 7, 8
        //      
        public const int PAGE_PREVIEW = 2;

        // Mặc định số lượng môi giới tối đa
        public const int MAX_AGENCY = 10;

        // tiền tố các đường link thân thiện
        public const string FRIENDLY_LINK_PRODUCT_DETAIL = "chi-tiet";

        public const string TAB_ID = "TAB_ID";

        // Kích thước của avatar
        public const int AVATAR_WIDTH = 320;
        public const int AVATAR_HEIGHT = 240;

        // Cookie user
        public const string COOKIES_USERNAME = "UserName";
        public const string COOKIES_USER_ID = "UserID";
        public const string COOKIES_ROLE_ID = "RoleID";
        public const string COOKIES_AVATAR = "Avatar";

        // TempData key
        public const string TEMP_USERNAME = "USERNAME";
        public const string TEMP_USER_ID = "USERID";
        public const string TEMP_AVATAR = "AVATAR";
        public const string TEMP_MESSAGE = "ADMIN_MESSAGE";
        public const string TEMP_REDIRECT = "REDIRECT";

        // SettingName (In table Setting)
        public const string SETTING_WEBSITE_TEMPDATA = "WebsiteData";
        public const string SETTING_WEBSITE = "Website";
        public const string SETTING_WEBSITE_EMAIL = "Email";
        public const string SETTING_WEBSITE_PHONE = "Phone";
        public const string SETTING_WEBSITE_URL = "Url";
        public const string SETTING_WEBSITE_ADDRESS = "Address";
        public const string SETTING_WEBSITE_WEBSITENAME = "WebsiteName";

        public const string SETTING_SOCIAL_NETWORK_TEMPDATA = "SocialNetworkData";
        public const string SETTING_SOCIAL_NETWORK = "SocialNetwork";
        public const string SETTING_SOCIAL_NETWORK_FACEBOOK = "Facebook";
        public const string SETTING_SOCIAL_NETWORK_SKYPE = "Skype";
        public const string SETTING_SOCIAL_NETWORK_GOOGLEPLUS = "GooglePlus";
        public const string SETTING_SOCIAL_NETWORK_YOUTUBE = "Youtube";
        

        // Query string (url parameter)
        public const string QUERY_PAGE = "page";
        public const string QUERY_CITY = "cityId";
        public const string QUERY_DISTRICT = "districtId";

        // Alert class
        public const string CLASS_ALERT_SUCCESS = "alert alert-success";
        public const string CLASS_ALERT_INFO = "alert alert-info";
        public const string CLASS_ALERT_WARNING = "alert alert-warning";
        public const string CLASS_ALERT_DANGER = "alert alert-danger";

        // Button
        public const string BUTTON_ADD = "Thêm mới";
        public const string BUTTON_UPDATE = "Cập nhật";
        public const string BUTTON_DELETE = "Xóa";
        public const string BUTTON_REFRESH = "Làm mới";

        // Confirm
        public const string CONFIRM_LOGOUT = "Bạn có thực sự muốn thoát không?";
        public const string CONFIRM_DELETE = "Bạn có chắc chắn muốn xóa \"{0} \" không?";

        // Message
        public const string MESSAGE_UPDATE_SUCCESS ="Cập nhật thành công!";
        public const string MESSAGE_UPDATE_ERROR = "Cập nhật thất bại!";
        public const string MESSAGE_NOT_DATA = "Chưa có dữ liệu";
        public const string MESSAGE_DEFAULT_ROLE = "Thành viên";

        public const string MESSAGE_PRODUCT_ORDER_OK = "Bạn đã đặt giá thành công!";
        public const string MESSAGE_PRODUCT_CHANGE_COST_OK = "Bạn đã sửa giá thành công!";
        public const string MESSAGE_PRODUCT_FOLLOW_OK = "Bạn đã đặt lệnh theo dõi nhà thành công!";
        public const string MESSAGE_REGISTER_AGENCY_OK = "Bạn đã đăng ký làm môi giới thành công!";
        public const string MESSAGE_NOT_FOUND = "Rất tiếc! Chúng tôi không tìm thấy kết quả phù hợp";
        public const string MESSAGE_IS_AGENCY = "Bạn đã là một môi giới!";
        
        // Errors
        public const string ERROR_EMAIL_EXISTS = "Email này đã tồn tại! Xin hãy chọn email khác.";
        public const string ERROR_ROLE_WRONG = "Quyền của bạn không thực hiện được chức năng này, vui lòng đăng nhập tài khoản khác.";
        public const string ERROR_USERNAME_EXISTS = "Username này đã tồn tại! Xin hãy chọn Username khác";
        public const string ERROR_NOT_LOGIN = "Bạn cần đăng nhập để thực hiện chức năng này!";
        public const string ERROR_NOT_UPDATE_INFO = "Bạn cần cập nhật thông tin để thực hiện chức năng này!";
        public const string ERROR_POSTER_NOT_EXISTS = "Người đăng bài không đúng!!!";
        public const string ERROR_INPUT_COST_LARGER = "Số tiền bạn nhập phải nhỏ hơn giá bán";
        public const string ERROR_INPUT_COST = "Số tiền bạn nhập vào không hợp lý.";

        public const string ERROR_PRODUCT_FORMAT = "Tiền nhập vào không đúng định dạng.";
        public const string ERROR_PRODUCT_LARGER = "Số tiền nhập vào phải nhỏ hơn giá bán";
        public const string ERROR_PRODUCT_SMALL = "Số tiền nhập vào quá nhỏ.";
        public const string ERROR_PRODUCT_NOT_EXISTS = "Sản phẩm không tồn tại.";
        public const string ERROR_PRODUCT_NOT_KNOW = "Đã có lỗi trong quá trình cập nhật. Vui lòng thử lại sau.";

        // Folders
        public const string FOLDER_UPLOADS = "Uploads";
        public const string FOLDER_IMAGES = "Images";
        public const string FOLDER_PRODUCTS = "Products";
        public const string FOLDER_PRODUCT_TEMPS = "Product_temps";
        public const string FOLDER_THUMBNAILS = "Thumbnails";

        // URL Image file
        public static char DIRSEPARATOR = System.IO.Path.DirectorySeparatorChar;
        public static char ALTDIRECTORYSEPARATORCHAR = System.IO.Path.AltDirectorySeparatorChar;
        public static string PHYSICAL_PATH =HttpContext.Current.Server.MapPath("~\\Content" + DIRSEPARATOR + FOLDER_UPLOADS + DIRSEPARATOR + FOLDER_IMAGES + DIRSEPARATOR);
        public static string IMAGE_PATH = "~/Content" + ALTDIRECTORYSEPARATORCHAR + FOLDER_UPLOADS + ALTDIRECTORYSEPARATORCHAR + FOLDER_IMAGES + ALTDIRECTORYSEPARATORCHAR;
        public static string IMAGE_PRODUCT_TEMP_PATH = "~/Content" + ALTDIRECTORYSEPARATORCHAR + FOLDER_UPLOADS + ALTDIRECTORYSEPARATORCHAR + FOLDER_IMAGES + ALTDIRECTORYSEPARATORCHAR + FOLDER_PRODUCT_TEMPS + ALTDIRECTORYSEPARATORCHAR;

        // Text hiển thị trên các trang, các chọn lựa
        public const string TEXT_STT = "STT"; // số thứ tự
        public const string TEXT_CODE = "Mã";

        public const string TEXT_LIST_CITY = "Danh sách tỉnh/ thành phố";
        public const string TEXT_LIST_DISTRICT = "Danh sách quận/ huyện";
        public const string TEXT_LIST_WARD = "Danh sách xã/ phường";
        public const string TEXT_LIST_REGION = "Danh sách vùng";
        public const string TEXT_LIST_ROAD = "Danh sách đường/ thôn làng";

        public const string TEXT_CHOOSE_CITY = "Chọn tỉnh/ thành phố";
        public const string TEXT_CHOOSE_DISTRICT = "Chọn quận/ huyện";
        public const string TEXT_CHOOSE_WARD = "Chọn xã/ phường";
        public const string TEXT_CHOOSE_REGION = "Chọn vùng";
        public const string TEXT_CHOOSE_ROAD = "Chọn đường, thôn làng";
        public const string TEXT_CHOOSE_ADDRESS = "Chọn địa chỉ";
        public const string TEXT_CHOOSE_HOUSE_TYPE = "Chọn kiểu nhà";

        public const string TEXT_NAME_CITY = "Tên tỉnh/ thành phố";
        public const string TEXT_NAME_DISTRICT = "Tên quận/ huyện";
        public const string TEXT_NAME_WARD = "Tên xã/ phường";
        public const string TEXT_NAME_REGION = "Tên vùng";
        public const string TEXT_NAME_ROAD = "Tên đường/ thôn làng";
        public const string TEXT_TITLE = "Tiêu đề";
        public const string TEXT_HOUSE_NUMBER = "Số nhà";

        public const string TUTORIAL_PERCEN_HOUSE = "(Phần trăm giá trị nhà)";
        public const string TUTORIAL_TRIEU_DONG = "Triệu đồng";
        public const string TUTORIAL_DOLA = "Đô la";
    }
}
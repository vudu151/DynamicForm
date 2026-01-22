# SƠ ĐỒ UML ACTIVITY DIAGRAM - DYNAMIC FORM

> **Mục tiêu**: Mô tả các quy trình nghiệp vụ chính của hệ thống DynamicForm dưới dạng UML Activity Diagram
>
> **Đối tượng**: Business Analyst, Product Owner, Developer, Tester
>
> **Công cụ**: PlantUML (có thể render tại http://www.plantuml.com/plantuml/uml/)

---

## 1. QUY TRÌNH TẠO VÀ THIẾT KẾ FORM

### 1.1. Mô tả

Quy trình này mô tả cách Admin tạo form mới, thiết kế các field, và kích hoạt version để người dùng có thể sử dụng.

**Luồng chính:**
1. Admin nhập thông tin form (Code, Name, Description, Version)
2. Hệ thống kiểm tra Form code - nếu đã có thì lấy, chưa có thì tạo mới
3. Hệ thống kiểm tra Version - nếu đã tồn tại thì **tự động tăng version** (1→2, 1.0.0→1.0.1)
4. Tạo Version mới và chuyển đến màn Designer
5. Admin thiết kế form (thêm field, sắp xếp, validation)
6. Lưu metadata form
7. Admin kích hoạt version → Form sẵn sàng sử dụng

**Đặc điểm quan trọng:**
- Nếu Form code đã tồn tại: Lấy Form đã có (không tạo mới)
- Nếu Version đã tồn tại: **Tự động tăng version** (1→2, 1.0.0→1.0.1) thay vì báo lỗi
- Hỗ trợ cả version số đơn giản (1, 2, 3...) và semantic version (1.0.0, 1.0.1...)

### 1.2. Activity Diagram

```plantuml
@startuml
title Quy trình Tạo và Thiết kế Form

start

:Admin nhập thông tin form\n(Code, Name, Description, Version);

:Kiểm tra Form code\nđã tồn tại?;

if (Form code đã tồn tại?) then (Có)
  :Lấy Form đã có;
else (Không)
  :Tạo Form mới;
endif

:Kiểm tra Version\nđã tồn tại?;

if (Version đã tồn tại?) then (Có)
  :Tự động tăng version\n(1→2, 1.0.0→1.0.1);
else (Không)
  :Sử dụng version đã nhập;
endif

:Tạo Version mới;

:Chuyển đến màn Designer;

:Admin thiết kế form\n(Thêm field, sắp xếp, validation);

:Lưu metadata form\n(Fields, Validations, Options);

:Admin kích hoạt version;

:Kích hoạt version này\nvà tắt các version khác;

:Form đã sẵn sàng sử dụng;

stop

@enduml
```

---

## 2. QUY TRÌNH ĐIỀN VÀ LƯU DỮ LIỆU FORM

### 2.1. Mô tả

Quy trình này mô tả cách người dùng (Doctor/Nurse) điền form, hệ thống validate và lưu dữ liệu vào database.

**Luồng chính:**
1. User mở form để điền → Hệ thống lấy metadata của version active
2. Render form động từ metadata
3. User điền dữ liệu vào các field
4. User bấm "Lưu" → Hệ thống validate dữ liệu (Required, Min/Max, Regex)
5. Nếu validation pass → Nhập ObjectId và ObjectType (liên kết với đối tượng nghiệp vụ)
6. Tạo SubmissionId mới và lưu dữ liệu vào Database
7. Hiển thị thông báo thành công

**Thông tin quan trọng:**
- **ObjectId**: ID của đối tượng nghiệp vụ liên quan (VD: DangKyId, BenhNhanId)
- **ObjectType**: Loại đối tượng (VD: PHIEU_KHAM, DANG_KY_KHAM)
- **SubmissionId**: Tự động tăng, dùng để nhóm các FormDataValue của cùng 1 submission
- Validation bao gồm: Required fields, Min/Max values, Regex patterns

### 2.2. Activity Diagram

```plantuml
@startuml
title Quy trình Điền và Lưu Dữ liệu Form

start

:User mở form để điền;

:Lấy metadata form\n(version active);

:Render form động từ metadata;

:User điền dữ liệu vào các field;

:User bấm nút "Lưu";

:Validate dữ liệu\n(Required, Min/Max, Regex);

if (Validation pass?) then (Không)
  :Hiển thị lỗi cho user sửa;
  stop
else (Có)
endif

:Nhập ObjectId và ObjectType\n(VD: DangKyId, PHIEU_KHAM);

:Tạo SubmissionId mới;

:Lưu dữ liệu vào Database\n(FormVersionId, ObjectId, ObjectType, Data);

:Trả về kết quả;

:Hiển thị thông báo thành công;

stop

@enduml
```

---

## 3. QUY TRÌNH XEM VÀ SỬA DỮ LIỆU ĐÃ LƯU

### 3.1. Mô tả

Quy trình này mô tả cách người dùng xem và sửa dữ liệu form đã được lưu trước đó.

### 3.2. Activity Diagram

```plantuml
@startuml
title Quy trình Xem và Sửa Dữ liệu đã lưu

start

:User chọn submission để xem/sửa;

:Web UI gọi API\nGET /api/formdata/{submissionId};

:API lấy dữ liệu theo SubmissionId;

:Query Database:\nSELECT * FROM FormDataValues\nWHERE SubmissionId=;

:Group by FieldCode\nvà tạo Dictionary;

:Trả về FormDataDto;

:Load metadata để render form;

:Hiển thị form với dữ liệu đã điền;

:User sửa đổi dữ liệu;

:User bấm nút "Lưu";

:Web UI gửi request\nPUT /api/formdata/{submissionId};

partition "Validation" {
  :Validate dữ liệu\n(giống quy trình 2);
  
  if (Validation pass?) then (Không)
    :Trả về lỗi validation;
    :Hiển thị lỗi cho user sửa;
    stop
  else (Có)
  endif
}

partition "Cập nhật dữ liệu" {
  :BEGIN TRANSACTION;
  
  :DELETE FormDataValues\nWHERE SubmissionId=;
  
  :INSERT FormDataValues\n(dữ liệu mới);
  
  :UPDATE ModifiedDate, ModifiedBy;
  
  :COMMIT TRANSACTION;
  
  :Trả về FormDataDto đã cập nhật;
  
  :Hiển thị thông báo cập nhật thành công;
  
  stop
}

@enduml
```

---

## 4. QUY TRÌNH TẠO VERSION MỚI (FORM ĐÃ TỒN TẠI)

### 4.1. Mô tả

Quy trình này mô tả cách hệ thống tự động tạo version mới khi user tạo form với code đã tồn tại.

### 4.2. Activity Diagram

```plantuml
@startuml
title Quy trình Tạo Version mới (Form đã tồn tại)

start

:Admin tạo form với code đã tồn tại;

:Web UI gửi request\nPOST /api/forms;

:API kiểm tra Form code đã tồn tại?;

if (Form code đã tồn tại?) then (Không)
  :Tạo form mới;
  stop
else (Có)
  :Không tạo form mới;
  
  :Lấy FormId của form đã tồn tại;
  
  :Web UI gửi request\nPOST /api/forms/{formId}/versions;
  
  :API kiểm tra version đã tồn tại?;
  
  if (Version đã tồn tại?) then (Có)
    partition "Auto-increment Version" {
      if (Version là số đơn giản?) then (Có - VD: "1")
        :Tìm MAX version số\n(1, 2, 3...);
        :Tạo version mới = MAX + 1;
      else (Không - VD: "1.0.0")
        :Tìm version cùng pattern\n(1.0.x);
        :Tăng phần cuối\n(1.0.0 → 1.0.1);
      endif
    }
  else (Không)
    :Sử dụng version đã nhập;
  endif
  
  :INSERT FormVersions\n(version đã tăng);
  
  :Trả về FormVersion đã tạo;
  
  :Chuyển đến Designer với version mới;
  
  stop
endif

@enduml
```

---

## 5. QUY TRÌNH VALIDATE DỮ LIỆU FORM

### 5.1. Mô tả

Quy trình này mô tả chi tiết cách hệ thống validate dữ liệu form trước khi lưu.

### 5.2. Activity Diagram

```plantuml
@startuml
title Quy trình Validate Dữ liệu Form

start

:Nhận dữ liệu form cần validate\n(FormVersionId, Data);

:Lấy metadata của version;

:Load danh sách FormFields;

:Load danh sách FieldValidations\n(sắp xếp theo Priority);

partition "Validate từng Field" {
  :Duyệt qua từng field trong metadata;
  
  :Kiểm tra field có IsRequired = true?;
  
  if (Field required?) then (Có)
    if (Dữ liệu có giá trị?) then (Không)
      :Thêm lỗi: "Trường này là bắt buộc";
    else (Có)
    endif
  else (Không)
  endif
  
  :Duyệt qua các Validation Rules\ncủa field (theo Priority);
  
  partition "Validate Rule" {
    if (RuleType = Required?) then (Có)
      if (Giá trị rỗng?) then (Có)
        :Thêm lỗi với ErrorMessage;
      else (Không)
      endif
    else (Không)
    endif
    
    if (RuleType = Min?) then (Có)
      if (Giá trị < Min?) then (Có)
        :Thêm lỗi với ErrorMessage;
      else (Không)
      endif
    else (Không)
    endif
    
    if (RuleType = Max?) then (Có)
      if (Giá trị > Max?) then (Có)
        :Thêm lỗi với ErrorMessage;
      else (Không)
      endif
    else (Không)
    endif
    
    if (RuleType = Range?) then (Có)
      if (Giá trị ngoài Range?) then (Có)
        :Thêm lỗi với ErrorMessage;
      else (Không)
      endif
    else (Không)
    endif
    
    if (RuleType = Regex?) then (Có)
      if (Giá trị không match Regex?) then (Có)
        :Thêm lỗi với ErrorMessage;
      else (Không)
      endif
    else (Không)
    endif
  }
  
  :Kiểm tra Field Conditions\n(hiển thị có điều kiện);
  
  if (Field có điều kiện?) then (Có)
    :Đánh giá điều kiện;
    if (Điều kiện thỏa mãn?) then (Có)
      :Field phải hiển thị;
      if (Field required nhưng không có giá trị?) then (Có)
        :Thêm lỗi;
      else (Không)
      endif
    else (Không)
      :Field không hiển thị\n(bỏ qua validation);
    endif
  else (Không)
  endif
}

if (Có lỗi validation?) then (Có)
  :Trả về ValidationResultDto\n(IsValid=false, Errors);
  stop
else (Không)
  :Trả về ValidationResultDto\n(IsValid=true, Errors=[]);
  stop
endif

@enduml
```

---

## 6. QUY TRÌNH TỔNG HỢP - LUỒNG SỬ DỤNG HỆ THỐNG

### 6.1. Mô tả

Quy trình tổng hợp mô tả luồng sử dụng hệ thống từ góc nhìn của các actor khác nhau.

### 6.2. Activity Diagram

```plantuml
@startuml
title Quy trình Tổng hợp - Luồng sử dụng hệ thống

start

partition "Admin" {
  :Tạo Form mới;
  :Thiết kế metadata;
  :Kích hoạt version;
}

partition "User (Doctor/Nurse)" {
  :Mở form để điền;
  :Điền dữ liệu;
  :Lưu dữ liệu;
}

partition "User xem/sửa" {
  :Chọn submission;
  :Xem dữ liệu;
  :Sửa dữ liệu (nếu cần);
  :Lưu cập nhật;
}

partition "Admin cập nhật" {
  :Tạo version mới;
  :Cập nhật metadata;
  :Kích hoạt version mới;
}

stop

@enduml
```

---

## 7. HƯỚNG DẪN SỬ DỤNG

### 7.1. Render PlantUML Diagrams

Có nhiều cách để render các sơ đồ PlantUML:

1. **Online**: 
   - Truy cập http://www.plantuml.com/plantuml/uml/
   - Copy code PlantUML và paste vào editor
   - Click "Submit" để xem kết quả

2. **VS Code Extension**:
   - Cài đặt extension "PlantUML"
   - Mở file .puml hoặc .md
   - Preview bằng cách nhấn Alt+D hoặc Command Palette: "PlantUML: Preview Current Diagram"

3. **Command Line**:
   ```bash
   # Cài đặt PlantUML
   # Windows: choco install plantuml
   # Mac: brew install plantuml
   # Linux: apt-get install plantuml
   
   # Render diagram
   plantuml UML-ACTIVITY-DIAGRAMS.md
   ```

4. **IntelliJ IDEA / PyCharm**:
   - Cài đặt plugin "PlantUML integration"
   - Mở file và preview trực tiếp

### 7.2. Export sang các định dạng khác

- **PNG/SVG**: Sử dụng PlantUML command line với option `-tpng` hoặc `-tsvg`
- **PDF**: Render sang PNG/SVG rồi convert sang PDF
- **HTML**: Sử dụng PlantUML server hoặc export từ VS Code

---

## 8. GHI CHÚ

### 8.1. Ký hiệu sử dụng

- **Start/Stop**: Điểm bắt đầu/kết thúc của quy trình
- **Activity**: Hành động được thực hiện
- **Decision**: Điểm quyết định (if/else)
- **Partition**: Nhóm các hoạt động liên quan
- **Fork/Join**: Song song hóa (nếu cần)

### 8.2. Các quy trình chính

1. **Tạo và Thiết kế Form**: Quy trình phức tạp nhất, bao gồm nhiều bước validation và transaction
2. **Điền và Lưu Dữ liệu**: Quy trình phổ biến nhất, có validation nghiêm ngặt
3. **Xem và Sửa Dữ liệu**: Tương tự quy trình 2 nhưng có thêm bước load dữ liệu cũ
4. **Tạo Version mới**: Quy trình tự động với logic tăng version thông minh
5. **Validate Dữ liệu**: Quy trình chi tiết về validation engine

### 8.3. Cải tiến có thể thêm

- Thêm Swimlane để phân biệt các actor (Admin, User, System)
- Thêm Exception handling flows
- Thêm Parallel processing cho các validation rules
- Thêm Timeout và Retry logic

---

**Tài liệu này mô tả đầy đủ các quy trình nghiệp vụ chính của hệ thống DynamicForm dưới dạng UML Activity Diagram. Cập nhật lần cuối: 2024-01-21**

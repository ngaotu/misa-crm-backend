CREATE TABLE misa_crm_2025.customer (
  customer_id char(36) NOT NULL DEFAULT (UUID()) COMMENT 'Khóa chính khách hàng',
  customer_code varchar(20) NOT NULL COMMENT '''Mã khách hàng tự sinh (KHyyyyMMxxxxxx)',
  customer_full_name varchar(128) NOT NULL COMMENT 'Họ và tên khách hàng',
  customer_tax_code varchar(20) DEFAULT NULL COMMENT 'Mã số thuế',
  customer_email varchar(100) DEFAULT NULL COMMENT 'Email khách hàng',
  customer_phone varchar(50) DEFAULT NULL COMMENT 'Số điện thoại khách hàng',
  customer_shipping_addr varchar(255) DEFAULT NULL COMMENT 'Địa chỉ giao hàng',
  customer_last_purchase_date datetime DEFAULT NULL COMMENT 'Ngày mua hàng gần nhất',
  is_deleted tinyint NOT NULL DEFAULT 0 COMMENT 'Cờ xóa mềm 0-chưa xóa, 1-đã xóa',
  customer_type varchar(100) DEFAULT NULL COMMENT 'Loại khách hàng (VIP, NBH01,...)',
  customer_purchased_items varchar(255) DEFAULT NULL COMMENT 'Hàng hóa đã mua (tổng quát: XEMAY)',
  customer_lastest_purchased_items varchar(255) DEFAULT NULL COMMENT 'Tên hàng hóa đã mua gần nhất',
  customer_avatar varchar(255) DEFAULT NULL,
  PRIMARY KEY (customer_id)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 354,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_0900_as_ci,
COMMENT = 'Danh mục khách hàng';

ALTER TABLE misa_crm_2025.customer
ADD INDEX ix_customer_customer_id (customer_id);

ALTER TABLE misa_crm_2025.customer
ADD UNIQUE INDEX uix_customer_customer_code (customer_code);

ALTER TABLE misa_crm_2025.customer
ADD UNIQUE INDEX uix_customer_email (customer_email);

ALTER TABLE misa_crm_2025.customer
ADD UNIQUE INDEX uix_customer_phone (customer_phone);
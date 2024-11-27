CREATE TABLE public.users (
	user_id BIGINT NOT NULL PRIMARY KEY, --
	username VARCHAR(50),                  -- 用户名
	password VARCHAR(50) NOT NULL,         -- 密码，不能为空
	created_at TIMESTAMP,                  -- 创建日期
	last_name VARCHAR(50) NOT NULL,        -- 姓，不能为空
	first_name VARCHAR(50) NOT NULL,       -- 名，不能为空
	avatar_thumbnail BYTEA                 -- 头像缩略图，存储二进制数据
);
	
CREATE TABLE public.user_history (
	user_id BIGINT NOT NULL,          -- 用户ID，不能为空
	seq_num BIGINT NOT NULL,          -- 序号，不能为空
	login_datetime TIMESTAMP,         -- 登录日期时间
	ip_address VARCHAR(255),          -- IP地址
	PRIMARY KEY (user_id, seq_num)    -- 复合主键
);

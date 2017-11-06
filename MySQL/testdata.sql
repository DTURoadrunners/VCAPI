USE DTU_Roadrunners;

set @useridDaniel = 's154163';
set @useridCecilie = 's153869';
set @useridThomas = 's154174';
set @projectName ='TEST';
set @err = 0;

# INSERT users
call DTU_Roadrunners.sp_create_user(@useridDaniel, 'Daniel', 'Plaetner-Cancela', '20722123');
call DTU_Roadrunners.sp_create_user(@useridCecilie, 'Cecilie', 'Lindberg', '61603272');
call DTU_Roadrunners.sp_create_user(@useridThomas, 'Thomas', 'Bech', '20768949');

# INSERT projects
call DTU_Roadrunners.sp_create_project('TEST', @useridDaniel, 'This is a test project', @err);
call DTU_Roadrunners.sp_create_project('Mubsus', @useridDaniel, 'Mubsus project', @err);
call DTU_Roadrunners.sp_create_project('Idiot project', @useridThomas, 'Projektet over alle', @err);
call DTU_Roadrunners.sp_create_project('DTU ROADUNNERS', @useridCecilie, 'RoadRunnersProject', @err);

# INSERT roles
call DTU_Roadrunners.sp_create_role('superadmin');
call DTU_Roadrunners.sp_create_role('admin');
call DTU_Roadrunners.sp_create_role('user');
call DTU_Roadrunners.sp_create_role('guest');

# INSERT category
call DTU_Roadrunners.sp_create_category('wheel');
call DTU_Roadrunners.sp_create_category('steel');
call DTU_Roadrunners.sp_create_category('misc');
call DTU_Roadrunners.sp_create_category('food');

# INSERT componentType
call DTU_Roadrunners.sp_create_componenttype('test', 1, 1, 51, 'lorem', @useridDaniel, 'blabla', @err);
call DTU_Roadrunners.sp_create_componenttype('test 2', 1, 2, 51, 'lorem', @useridDaniel, 'blabla', @err);
call DTU_Roadrunners.sp_create_componenttype('test 3', 1, 3, 51, 'lorem', @useridDaniel, 'blabla', @err);
call DTU_Roadrunners.sp_create_componenttype('test 4', 1, 3, 51, 'lorem', @useridCecilie, 'blabla', @err);
call DTU_Roadrunners.sp_create_componenttype('test dada', 2, 3, 51, 'lorem', @useridCecilie, 'blabla', @err);
call DTU_Roadrunners.sp_create_componenttype('test da', 3, 3, 51, 'lorem', @useridCecilie, 'blabla', @err);

# INSERT component
call DTU_Roadrunners.createComponent(1, 'this is being used by Thomas - lel', 'a comment dsd', @useridThomas, 'log comment');
call DTU_Roadrunners.createComponent(2, 'this is being used by Thomas - dsds', 'a comment dads', @useridThomas, 'log comment');
call DTU_Roadrunners.createComponent(3, 'this is being used by Thomas - dsds', 'a comment fdfdf', @useridThomas, 'log comment');
call DTU_Roadrunners.createComponent(3, 'this is being used by Cecilie - dsds', 'a comment fdfdadf', @useridCecilie, 'log comment');
call DTU_Roadrunners.createComponent(3, 'this is being used by Daniel - dsds', 'a comment fddadfdf', @useridDaniel, 'log comment');

# INSERT document





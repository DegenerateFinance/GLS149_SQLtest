SELECT * FROM gls149_test.univerre LIMIT 50;

REPLACE INTO gls149_test.univerre (UVRTabla, UVRKeyN01, UVRKeyN02) VALUES ('K10CINT149_1', 1, 0);

-- 2. Registro que graba IDESAI: parcel data
INSERT INTO gls149_test.univerre 
(UVRTabla, 
UVRKeyN01, 
UVRTxt01, 
UVRNum01, 
UVRNum02, 
UVRBar01, 
UVRBar02) 
VALUES 
('K10CINT149_1', 
0, 
'Código de barras ', 
200, 
222, 
333, 
444);

SELECT * FROM gls149_test.univerre WHERE UVRTabla = 'K10CINT149_1' AND UVRKeyN01 = 0 LIMIT 50;
SELECT * FROM gls149_test.univerre LIMIT 50;

SELECT UVRTabla, UVRKeyN01 FROM gls149_test.Univerre WHERE UVRTabla = 'K10CINT149_1' AND UVRKeyN01 = 0;

DELETE FROM gls149_test.univerre WHERE 
UVRTabla= 'K10CINT149_1' AND
UVRKeyN01 = 0;

-- 3.

DELETE FROM gls149_test.univerre WHERE 
UVRTabla= 'K10CINT149_1' AND
UVRKeyN01 = 1 AND
UVRKeyN02 = 0 AND
UVRKeyC01 = 'n' AND
UVRKeyC02 = 'n';

DELETE FROM gls149_test.univerre WHERE 
UVRTabla= 'K10CINT149_1' AND
UVRKeyN01 = 1 AND
UVRKeyN02 = 0;

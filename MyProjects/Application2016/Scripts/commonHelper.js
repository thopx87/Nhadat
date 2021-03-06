function ListPlaceJson(parentId, toControlId1, toControlId2) {
    if (parentId != null && parentId != "") {
        $.ajax({
            url: '/Admin/Base/ListPlaceJson/',
            data: JSON.stringify({ parentId: parentId }),
            contentType: 'application/json; charset=UTF-8',
            type: 'POST',
            dataType: 'json'
        })
            .success(function (result) {
                $(toControlId1).empty();
                if (result.length > 0) {
                    // Thêm vào Control 1
                    for (var i = 0; i < result.length; i++) {
                        $(toControlId1).append("<option value=" + result[i].Id + ">" + result[i].Text + "</option>");
                    }

                    // Thêm vào control 2 theo ID đầu tiên của trontrol 1
                    if (toControlId2 != null && toControlId2 != "") {
                        $(toControlId2).empty();
                        ListPlaceJson(result[0].Id, toControlId2, "");
                    }
                }
            })
            .error(function (xhr, status) {
                alert(status);
            });
    } else {
        $(toControlId1).empty();
    }
}

function ListRegionJson(cityId, districtId, regionId, wardId) {
    // Get city value
    var cityVal = $('#'+cityId).val();
    // Get district value
    var districtVal = $('#'+districtId).val()
    if (cityVal != null && cityVal != "") {
        $.ajax({
            url: '/Admin/Base/ListRegionJson/',
            data: JSON.stringify({ cityId: cityVal, districtId: districtVal }),
            contentType: 'application/json; charset=UTF-8',
            type: 'POST',
            dataType: 'json'
        })
        .success(function (result) {
            $(regionId).empty();
            for (var i = 0; i < result.length; i++) {
                $(regionId).append("<option value=" + result[i].Id + ">" + result[i].Text + "</option>");
            }
        })
        .error(function (xhr, status) {
            alert(status);
        });

        if (wardId != null) {
            ListPlaceJson(districtVal, wardId, null);
        }
    } else {
        $(regionId).empty();
    }
}

function ListPlaceByRegionJson(regionId, toControlId1) {
    if (regionId != null && regionId != "") {
        $.ajax({
            url: '/Admin/Base/ListPlaceByRegions/',
            data: JSON.stringify({ regionIds: regionId }),
            contentType: 'application/json; charset=UTF-8',
            type: 'POST',
            dataType: 'json'
        })
            .success(function (result) {
                // Thêm vào Control 1
                $(toControlId1).empty();
                for (var i = 0; i < result.length; i++) {
                    $(toControlId1).append("<option value=" + result[i].Id + ">" + result[i].Text + "</option>");
                }

            })
            .error(function (xhr, status) {
                alert(status);
            });
    } else {
        $(toControlId1).empty();
    }
}

function ListUserByRole(roleId, regionId, toControl) {
    var regionVal = $(regionId).length ? $(regionId).val() : 0;
    if (roleId != null && roleId != "") {
        $.ajax({
            url: '/Admin/Base/ListUserByRole/',
            data: JSON.stringify({ roleId: roleId, regionId: regionVal, getItem: true }),
            contentType: 'application/json; charset=UTF-8',
            type: 'POST',
            dataType: 'json'
        })
            .success(function (result) {
                $(toControl).html('');
                var strHTML = "";
                for (var i = 0; i < result.length; i++) {
                    strHTML += "<div class=\"checkbox\">";
                    strHTML += "<label>";
                    strHTML += "<input data-val=true id=\"ListUser_" + i + "__Id\" name=\"ListUser[" + i + "].Id\" type=hidden value=" + result[i].Id + " >";
                    strHTML += "<input type=\"checkbox\" name=\"ListUser[" + i + "].Checked\" value=true data-val=true id=\"ListUser_" + result[i].Id + "\" class=\"ace ace-switch ace-switch-3\"/>";
                    strHTML += "<span class=\"lbl\"><label for=\"ListUser_" + result[i].Id + "\">" + result[i].Text + "</label></span>";
                    strHTML += "<input name=\"ListUser[" + i + "].Checked\" type=\"hidden\" value=\"false\">";
                    strHTML += "</lable>";
                    strHTML += "</div>";
                }
                $(toControl).html(strHTML);
            })
            .error(function (xhr, status) {
                alert(status);
            });
    }
    }

    var ChuSo = new Array(" không ", " một ", " hai ", " ba ", " bốn ", " năm ", " sáu ", " bảy ", " tám ", " chín ");
    var Tien = new Array("", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ");

    //1. Hàm đọc số có ba chữ số;
    function DocSo3ChuSo(baso) {
        var tram;
        var chuc;
        var donvi;
        var KetQua = "";
        tram = parseInt(baso / 100);
        chuc = parseInt((baso % 100) / 10);
        donvi = baso % 10;
        if (tram == 0 && chuc == 0 && donvi == 0) return "";
        if (tram != 0) {
            KetQua += ChuSo[tram] + " trăm ";
            if ((chuc == 0) && (donvi != 0)) KetQua += " linh ";
        }
        if ((chuc != 0) && (chuc != 1)) {
            KetQua += ChuSo[chuc] + " mươi";
            if ((chuc == 0) && (donvi != 0)) KetQua = KetQua + " linh ";
        }
        if (chuc == 1) KetQua += " mười ";
        switch (donvi) {
            case 1:
                if ((chuc != 0) && (chuc != 1)) {
                    KetQua += " mốt ";
                }
                else {
                    KetQua += ChuSo[donvi];
                }
                break;
            case 5:
                if (chuc == 0) {
                    KetQua += ChuSo[donvi];
                }
                else {
                    KetQua += " lăm ";
                }
                break;
            default:
                if (donvi != 0) {
                    KetQua += ChuSo[donvi];
                }
                break;
        }
        return KetQua;
    }

    //2. Hàm đọc số thành chữ (Sử dụng hàm đọc số có ba chữ số)

    function DocTienBangChu(SoTien) {
        var lan = 0;
        var i = 0;
        var so = 0;
        var KetQua = "";
        var tmp = "";
        var ViTri = new Array();
        if (SoTien < 0) return "Số tiền âm !";
        if (SoTien == 0) return "Không đồng !";
        if (SoTien > 0) {
            so = SoTien;
        }
        else {
            so = -SoTien;
        }
        if (SoTien > 9999999999999999) {
            //SoTien = 0;
            return "Số quá lớn!";
        }
        ViTri[5] = Math.floor(so / 1000000000000000);
        if (isNaN(ViTri[5]))
            ViTri[5] = "0";
        so = so - parseFloat(ViTri[5].toString()) * 1000000000000000;
        ViTri[4] = Math.floor(so / 1000000000000);
        if (isNaN(ViTri[4]))
            ViTri[4] = "0";
        so = so - parseFloat(ViTri[4].toString()) * 1000000000000;
        ViTri[3] = Math.floor(so / 1000000000);
        if (isNaN(ViTri[3]))
            ViTri[3] = "0";
        so = so - parseFloat(ViTri[3].toString()) * 1000000000;
        ViTri[2] = parseInt(so / 1000000);
        if (isNaN(ViTri[2]))
            ViTri[2] = "0";
        ViTri[1] = parseInt((so % 1000000) / 1000);
        if (isNaN(ViTri[1]))
            ViTri[1] = "0";
        ViTri[0] = parseInt(so % 1000);
        if (isNaN(ViTri[0]))
            ViTri[0] = "0";
        if (ViTri[5] > 0) {
            lan = 5;
        }
        else if (ViTri[4] > 0) {
            lan = 4;
        }
        else if (ViTri[3] > 0) {
            lan = 3;
        }
        else if (ViTri[2] > 0) {
            lan = 2;
        }
        else if (ViTri[1] > 0) {
            lan = 1;
        }
        else {
            lan = 0;
        }
        for (i = lan; i >= 0; i--) {
            tmp = DocSo3ChuSo(ViTri[i]);
            KetQua += tmp;
            if (ViTri[i] > 0) KetQua += Tien[i];
            if ((i > 0) && (tmp.length > 0)) KetQua += ','; //&& (!string.IsNullOrEmpty(tmp))
        }
        if (KetQua.substring(KetQua.length - 1) == ',') {
            KetQua = KetQua.substring(0, KetQua.length - 1);
        }
        KetQua = KetQua.substring(1, 2).toUpperCase() + KetQua.substring(2);

        return KetQua + ' đồng'; //.substring(0, 1);//.toUpperCase();// + KetQua.substring(1);
    }
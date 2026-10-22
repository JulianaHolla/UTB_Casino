// Client-side counterparts of the custom [MinimumAge] and [GreaterThanOrEqualTo] attributes.
(function ($) {
  $.validator.addMethod("minimumage", function (value, element, years) {
    if (!value) return true;
    var dob = new Date(value), today = new Date();
    var age = today.getFullYear() - dob.getFullYear();
    var m = today.getMonth() - dob.getMonth();
    if (m < 0 || (m === 0 && today.getDate() < dob.getDate())) age--;
    return age >= years;
  });
  $.validator.unobtrusive.adapters.add("minimumage", ["years"], function (options) {
    options.rules["minimumage"] = parseInt(options.params.years, 10);
    options.messages["minimumage"] = options.message;
  });

  $.validator.addMethod("greaterthanorequalto", function (value, element, param) {
    var other = $(param).val();
    if (value === "" || other === "") return true;
    return parseFloat(value) >= parseFloat(other);
  });
  $.validator.unobtrusive.adapters.add("greaterthanorequalto", ["other"], function (options) {
    options.rules["greaterthanorequalto"] = "#" + options.params.other;
    options.messages["greaterthanorequalto"] = options.message;
  });
})(jQuery);

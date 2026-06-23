using DurjoyBDNews24.Application.DTOs.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurjoyBDNews24.Application.Validators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("ইমেইল দিতে হবে")
                .EmailAddress().WithMessage("সঠিক ইমেইল দিন");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("পাসওয়ার্ড দিতে হবে")
                .MinimumLength(8).WithMessage("পাসওয়ার্ড কমপক্ষে ৮ অক্ষর হতে হবে");
        }
    }

    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.FullNameBn)
                .NotEmpty().WithMessage("বাংলায় নাম দিতে হবে");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("ইমেইল দিতে হবে")
                .EmailAddress().WithMessage("সঠিক ইমেইল দিন");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("পাসওয়ার্ড দিতে হবে")
                .MinimumLength(8).WithMessage("পাসওয়ার্ড কমপক্ষে ৮ অক্ষর হতে হবে")
                .Matches("[A-Z]").WithMessage("পাসওয়ার্ডে কমপক্ষে একটি বড় হাতের অক্ষর থাকতে হবে")
                .Matches("[0-9]").WithMessage("পাসওয়ার্ডে কমপক্ষে একটি সংখ্যা থাকতে হবে");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("ফোন নম্বর দিতে হবে")
                .Matches(@"^01[3-9]\d{8}$").WithMessage("সঠিক বাংলাদেশি ফোন নম্বর দিন");
        }
    }
}

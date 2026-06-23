using DurjoyBDNews24.Application.DTOs.Article;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurjoyBDNews24.Application.Validators
{
    public class CreateArticleValidator : AbstractValidator<CreateArticleDto>
    {
        public CreateArticleValidator()
        {
            RuleFor(x => x.TitleBn)
                .NotEmpty().WithMessage("বাংলা শিরোনাম দিতে হবে")
                .MaximumLength(500).WithMessage("শিরোনাম ৫০০ অক্ষরের বেশি হবে না");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("ইংরেজি শিরোনাম দিতে হবে")
                .MaximumLength(500).WithMessage("Title cannot exceed 500 characters");

            RuleFor(x => x.ContentBn)
                .NotEmpty().WithMessage("বাংলা বিষয়বস্তু দিতে হবে");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("বিভাগ নির্বাচন করতে হবে");
        }
    }

    public class UpdateArticleValidator : AbstractValidator<UpdateArticleDto>
    {
        public UpdateArticleValidator()
        {
            Include(new CreateArticleValidator());
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Article ID সঠিক নয়");
        }
    }
}

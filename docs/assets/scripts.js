$(function () {
  $('[data-toggle="tooltip"]').tooltip()

  const contextualCSharpKeywords = new Set([
    'record',
    'required',
    'init',
    'with',
    'and',
    'or',
    'not',
    'file',
    'scoped'
  ])

  document.querySelectorAll('.language-csharp code span.n, .language-csharp code span.nc').forEach((token) => {
    if (contextualCSharpKeywords.has(token.textContent.trim())) {
      token.classList.add('csharp-contextual-keyword')
    }
  })
})

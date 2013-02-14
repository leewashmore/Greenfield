using System;

namespace Aims.Core
{
	public interface ISecurityIdToPortfolioIdResolver
	{
		String TryResolveToPortfolioId(String securityId);
	}
}

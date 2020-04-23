if (globalObj != null) {
	function copyTo(src: any, target: any) {
		for (var n in src)
			if (src.hasOwnProperty(n))
				target[n] = src[n];
	}
	globalObj.Q ? copyTo(Q, globalObj.Q) : globalObj.Q = Q;
	globalObj.Serenity ? copyTo(Serenity, globalObj.Serenity) : globalObj.Serenity = Serenity;
}


